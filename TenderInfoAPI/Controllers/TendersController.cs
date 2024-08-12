using System.Net;
using TenderInfoAPI.DTOs;
using TenderInfoAPI.Requests;
using TenderInfoAPI.Middleware;
using Microsoft.AspNetCore.Mvc;
using TenderInfoAPI.Services;

namespace TenderInfoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TendersController : ControllerBase
{
    private readonly ITenderService _tenderService;

    public TendersController(ITenderService tenderService)
    {
        _tenderService = tenderService;
    }

    /// <summary>
    /// Retrieves a list of tenders with optional filtering and ordering.
    /// </summary>
    /// <param name="request">The request containing filtering and ordering options.</param>
    /// <returns>A list of tenders.</returns>
    /// <response code="200">Returns the list of tenders</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TenderDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTenders([FromQuery] GetTendersRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tenders = await _tenderService.GetTendersAsync(request);
        return Ok(tenders);
    }

    /// <summary>
    /// Retrieves a specific tender by ID.
    /// </summary>
    /// <param name="id">The ID of the tender.</param>
    /// <returns>The tender with the specified ID.</returns>
    /// <response code="200">Returns the tender</response>
    /// <response code="404">If the tender is not found</response>
    /// <response code="400">If a given ID is null or empty</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TenderDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTenderById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new ErrorDetails
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Tender ID cannot be null or empty."
            });
        }

        var tender = await _tenderService.GetTenderByIdAsync(id);
        return tender != null ? Ok(tender) : NotFound(new ErrorDetails
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Message = $"Tender with ID {id} not found."
        });
    }

    /// <summary>
    /// Retrieves a list of tenders filtered by supplier ID.
    /// </summary>
    /// <param name="request">The request containing the supplier ID.</param>
    /// <returns>A list of tenders associated with the specified supplier.</returns>
    /// <response code="200">Returns the list of tenders</response>
    /// <response code="404">If no tenders are found for the supplier</response>
    /// <response code="400">If a given ID is null or empty</response>
    [HttpGet("by-supplier")]
    [ProducesResponseType(typeof(IEnumerable<TenderDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTendersBySupplier([FromQuery] GetTendersBySupplierRequest request)
    {
        var tenders = await _tenderService.GetTendersBySupplierAsync(request);
        return tenders != null && tenders.Any()
            ? Ok(tenders)
            : NotFound(new ErrorDetails
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = $"No tenders found for supplier ID {request.SupplierId}."
            });
    }
}
