﻿using FluentValidation;

namespace BugTrackerApi.Features.Projects.DeleteProjectEndpoint;

public class DeleteProjectRequest
{
    public int ProjectId { get; set; }
}

public class DeleteProjectRequestValidator : AbstractValidator<DeleteProjectRequest>
{

}
