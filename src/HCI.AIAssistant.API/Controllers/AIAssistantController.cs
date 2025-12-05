using HCI.AIAssistant.API.Models.DTOs;
using HCI.AIAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HCI.AIAssistant.API.Controllers;

public class AIAssistantController : ControllerBase
{
    private readonly IAIAssistantService _aIAssistantService;
    private readonly IParametricFunctions _parametricFunctions;
    private readonly ISecretsService _secretsService;
    private readonly IAppConfigurationsService _appConfigurationsService;

    public AIAssistantController(
        IAIAssistantService aIAssistantService,
        IParametricFunctions parametricFunctions,
        ISecretsService secretsService,
        IAppConfigurationsService appConfigurationsService
    )
    {
        _aIAssistantService = aIAssistantService;
        _parametricFunctions = parametricFunctions;
        _secretsService = secretsService;
        _appConfigurationsService = appConfigurationsService;
    }

    [HttpPost("message")]
    [ProducesResponseType(typeof(AIAssistantControllerPostMessageResponseDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
    public async Task<ActionResult> PostMessage([FromBody] AIAssistantControllerPostMessageRequestDTO request)
    {
        if (!_parametricFunctions.ObjectExistsAndHasNoNullPublicProperties(request))
        {
            return BadRequest(
                new ErrorResponseDTO()
                {
                    TextErrorTitle = "AtLeastOneNullParameter",
                    TextErrorMessage = "Some parameters are null/missing.",
                    TextErrorTrace = _parametricFunctions.GetCallerTrace()
                }
            );
        }

#pragma warning disable CS8604
        string textMessageResponse = await _aIAssistantService.SendMessageAndGetResponseAsync(request.TextMessage);
#pragma warning restore CS8604

        AIAssistantControllerPostMessageResponseDTO response = new()
        {
            TextMessage = textMessageResponse
        };

        return Ok(response);
    }
}

