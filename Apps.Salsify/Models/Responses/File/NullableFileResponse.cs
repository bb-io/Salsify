using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Responses.File;

public record NullableFileResponse(FileReference? Content);