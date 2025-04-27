using Amazon.CDK;

namespace NoteFlow.CDK;

sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        new NoteFlowCdkStack(app, "NoteFlowCdkStack", new StackProps
        {
            
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
            }
            
        });
        app.Synth();
    }
}
