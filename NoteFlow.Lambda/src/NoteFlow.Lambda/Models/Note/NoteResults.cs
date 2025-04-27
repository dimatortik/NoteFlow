using Amazon.Runtime;
using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;
using NoteFlow.Application.UseCases.Users.Get;

namespace NoteFlow.Lambda.Models.Note;

public class CreateNoteResult : BaseResult
{
    public required NoteResponse Note { get; set; }
}
public class UpdateNoteResult : BaseResult
{
    public required NoteResponse Note { get; set; }
}
public class GetNoteResult : BaseResult
{
    public required NoteResponse Note { get; set; }
}
public class GetUserNotesResult : BaseResult
{
    public required PaginatedResult<NoteResponse> Notes { get; set; }
}
public class DeleteNoteResult : BaseResult
{
    public required string NoteId { get; set; }
}