using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;
using Code = Amazon.CDK.AWS.Lambda.Code;

namespace NoteFlow.CDK
{
    public class NoteFlowCdkStack : Stack
    {
        public NoteFlowCdkStack(Construct scope, string id, IStackProps props = null) 
            : base(scope, id, props)
        {
            var tables = CreateDynamoDbTables();
            
            var lambdaRole = CreateLambdaRole(tables);
            
            var graphqlLambda = CreateGraphQLLambda(lambdaRole);
            
            var api = CreateAppSyncApi();
            var schema = CreateGraphQLSchema(api);
            
            var lambdaDataSource = CreateLambdaDataSource(api, graphqlLambda);
            
            CreateResolvers(api, lambdaDataSource);
            
            new CfnOutput(this, "GraphQLApiUrl", new CfnOutputProps
            {
                Value = api.AttrGraphQlUrl,
                Description = "URL of the AppSync GraphQL API",
            });
        }
        
        private List<Table> CreateDynamoDbTables()
        {
            var usersTable = new Table(this, "UsersTable", new TableProps
            {
                TableName = "users",
                PartitionKey = new Attribute { Name = "pk", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.DESTROY
            });
            
            var notesTable = new Table(this, "NotesTable", new TableProps
            {
                TableName = "notes",
                PartitionKey = new Attribute { Name = "pk", Type = AttributeType.STRING },
                SortKey = new Attribute { Name = "sk", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.DESTROY
            });
            
            notesTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "UserId-CreatedAt-Note-index",
                PartitionKey = new Attribute { Name = "pk", Type = AttributeType.STRING },
                SortKey = new Attribute { Name = "CreatedAt", Type = AttributeType.STRING },
                ProjectionType = ProjectionType.ALL
            });
            
            return
            [
                usersTable,
                notesTable
            ];
            
        }
        
        private Role CreateLambdaRole(List<Table> tables)
        {
            var role = new Role(this, "NoteFlowLambdaRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });
            
            role.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            role.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AWSXRayDaemonWriteAccess"));
            
            foreach (var table in tables)
            {
                table.GrantReadWriteData(role);
            }
            
            return role;
        }
        
        private Function CreateGraphQLLambda(Role role)
        {
            return new Function(this, "NoteFlowGraphQLLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "NoteFlow.Lambda::NoteFlow.Lambda.GraphQlHandler::Handle",
                Code = Code.FromAsset("../NoteFlow.Lambda/src/NoteFlow.Lambda/bin/Release//net8.0/linux-x64/publish"),
                Role = role,
                Timeout = Duration.Seconds(30),
                Tracing = Tracing.ACTIVE,
                MemorySize = 512,
                Environment = new Dictionary<string, string>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Production",
                    ["LAMBDA_NET_SERIALIZER_DEBUG"] = "true",
                    ["DOTNET_SYSTEM_GLOBALIZATION_INVARIANT"] = "true"
                }
            });
        }
        
        private CfnGraphQLApi CreateAppSyncApi()
        {
            var api = new CfnGraphQLApi(this, "NoteFlowAPI", new CfnGraphQLApiProps
            {
                Name = "NoteFlowAPI",
                AuthenticationType = "API_KEY", 
                XrayEnabled = true 
            });
            
            var apiKey = new CfnApiKey(this, "NoteFlowApiKey", new CfnApiKeyProps
            {
                ApiId = api.AttrApiId
            });
            
            return api;
        }
        
        private CfnGraphQLSchema CreateGraphQLSchema(CfnGraphQLApi api)
        {
            return new CfnGraphQLSchema(this, "NoteFlowSchema", new CfnGraphQLSchemaProps
            {
                ApiId = api.AttrApiId,
                Definition = File.ReadAllText("./schema.graphql")
            });
        }
        
        private CfnDataSource CreateLambdaDataSource(CfnGraphQLApi api, Function lambda)
        {
            var appSyncServiceRole = new Role(this, "AppSyncServiceRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("appsync.amazonaws.com")
            });
            
            lambda.GrantInvoke(appSyncServiceRole);
            
            return new CfnDataSource(this, "LambdaDataSource", new CfnDataSourceProps
            {
                ApiId = api.AttrApiId,
                Name = "NoteFlowLambdaDataSource",
                Type = "AWS_LAMBDA",
                LambdaConfig = new CfnDataSource.LambdaConfigProperty
                {
                    LambdaFunctionArn = lambda.FunctionArn
                },
                ServiceRoleArn = appSyncServiceRole.RoleArn
            });
        }
        
        private void CreateResolvers(CfnGraphQLApi api, CfnDataSource dataSource)
        {
            CreateResolver(api.AttrApiId, "Query", "getUser", dataSource.AttrName);
            CreateResolver(api.AttrApiId, "Query", "getNote", dataSource.AttrName);
            CreateResolver(api.AttrApiId, "Query", "getUserNotes", dataSource.AttrName);
            
            CreateResolver(api.AttrApiId, "Mutation", "createUser", dataSource.AttrName);
            CreateResolver(api.AttrApiId, "Mutation", "updateUser", dataSource.AttrName);
            CreateResolver(api.AttrApiId, "Mutation", "deleteUser", dataSource.AttrName);
            CreateResolver(api.AttrApiId, "Mutation", "createNote", dataSource.AttrName);
            CreateResolver(api.AttrApiId, "Mutation", "updateNote", dataSource.AttrName);
            CreateResolver(api.AttrApiId, "Mutation", "deleteNote", dataSource.AttrName);
        }
        
        private void CreateResolver(string apiId, string typeName, string fieldName, string dataSourceName)
        {
            new CfnResolver(this, $"{typeName}{fieldName}Resolver", new CfnResolverProps
            {
                ApiId = apiId,
                TypeName = typeName,
                FieldName = fieldName,
                DataSourceName = dataSourceName,
                RequestMappingTemplate = CreateRequestTemplate(fieldName),
                ResponseMappingTemplate = "$util.toJson($ctx.result)"
            });
        }
        
        private string CreateRequestTemplate(string fieldName)
        {
            return @"{
                ""version"": ""2018-05-29"",
                ""operation"": ""Invoke"",
                ""payload"": {
                    ""field"": """ + fieldName + @""",
                    ""arguments"": $util.toJson($ctx.arguments),
                    ""identity"": $util.toJson($ctx.identity),
                    ""source"": $util.toJson($ctx.source),
                    ""info"": {
                        ""fieldName"": ""$ctx.info.fieldName"",
                        ""parentTypeName"": ""$ctx.info.parentTypeName"",
                        ""variables"": $util.toJson($ctx.info.variables),
                        ""selectionSetList"": $util.toJson($ctx.info.selectionSetList)
                    }
                }
            }";
        }
    }
}