using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.Features.Contacts.Commands.CreateContact;
using NexaCRM.Application.Features.Contacts.Commands.DeleteContact;
using NexaCRM.Application.Features.Contacts.Commands.LinkContactToLead;
using NexaCRM.Application.Features.Contacts.Commands.UpdateContact;
using NexaCRM.Application.Features.Contacts.Queries.GetContactById;
using NexaCRM.Application.Features.Contacts.Queries.GetContacts;

namespace NexaCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public ContactsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    // POST api/contacts
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateContactRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateContactCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            _currentUser.TenantId,
            _currentUser.UserId,
            request.Phone,
            request.Company,
            request.JobTitle,
            request.LinkedIn,
            request.Address,
            request.LeadId), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById),
                new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/contacts
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? company,
        [FromQuery] Guid? leadId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetContactsQuery(
            _currentUser.TenantId, search, company, leadId, page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // GET api/contacts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetContactByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    // PUT api/contacts/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateContactRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateContactCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Company,
            request.JobTitle,
            request.LinkedIn,
            request.Address), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // DELETE api/contacts/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteContactCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // PATCH api/contacts/{id}/link-lead
    [HttpPatch("{id:guid}/link-lead")]
    public async Task<IActionResult> LinkToLead(
        Guid id,
        [FromBody] LinkContactToLeadRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LinkContactToLeadCommand(id, request.LeadId),
            cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }
}

// Request Models 
public record CreateContactRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone = null,
    string? Company = null,
    string? JobTitle = null,
    string? LinkedIn = null,
    string? Address = null,
    Guid? LeadId = null);

public record UpdateContactRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone = null,
    string? Company = null,
    string? JobTitle = null,
    string? LinkedIn = null,
    string? Address = null);

public record LinkContactToLeadRequest(Guid LeadId);