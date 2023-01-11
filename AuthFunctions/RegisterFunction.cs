using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuthFunctions.Data.UnitOfWorks;
using AuthFunctions.Domain.Dtos;
using AuthFunctions.Domain.Models.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AuthFunctions
{
    public class RegisterFunction
    {
        private readonly ILogger<RegisterFunction> _logger;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterFunction(ILogger<RegisterFunction> log, IValidator<RegisterDto> registerValidator, IUnitOfWork unitOfWork)
        {
            _logger = log;
            _registerValidator = registerValidator;
            _unitOfWork = unitOfWork;
        }

        [FunctionName("RegisterFunction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Register" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(RegisterDto), Description = "Parameters", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(RegisterResponseDto), Description = "The Created response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(RegisterResponseDto), Description = "The Bad Request response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "application/json", bodyType: typeof(RegisterResponseDto), Description = "The Already Exists response")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function {nameof(RegisterFunction)} processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var dto = JsonConvert.DeserializeObject<RegisterDto>(requestBody);

            var validationResult = _registerValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                var errorResponse = new RegisterResponseDto { Errors = validationResult.Errors.Select(s => new ErrorResponseDto { Message = s.ErrorMessage }).ToList() };
                _logger.LogError($"{nameof(RegisterFunction)}: Validation error: {JsonConvert.SerializeObject(errorResponse)}");
                return new BadRequestObjectResult(errorResponse);
            }

            var existingUser = await _unitOfWork.UserRepository.GetAsync(u =>
                u.UserName == dto.UserName ||
                u.Email == dto.Email ||
                u.UserName == dto.Email ||
                u.Email == dto.UserName);
            if (existingUser != null)
            {
                var errorResponse = new RegisterResponseDto { Errors = new List<ErrorResponseDto> { new ErrorResponseDto { Message = $"The {nameof(dto.UserName)} or {nameof(dto.Email)} already exists." } } };
                _logger.LogError($"{nameof(RegisterFunction)}: The user {dto.UserName}, or email {dto.Email} already exists.");
                return new ConflictObjectResult(errorResponse);
            }

            var newUser = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password),
            };

            if (!string.IsNullOrWhiteSpace(dto.FirstName) || !string.IsNullOrWhiteSpace(dto.LastName))
            {
                newUser.Profile = new Profile { FirstName = dto.FirstName, LastName = dto.LastName };
            }

            newUser.Roles.Add(new Role { Name = "User" });

            await _unitOfWork.UserRepository.AddAsync(newUser);

            await _unitOfWork.CompleteAsync();

            var response = new RegisterResponseDto { Message = "The user is created." };
            _logger.LogInformation($"{nameof(RegisterFunction)}: The user {dto.UserName}, with email {dto.Email} is created.");

            return new CreatedResult("", response);
        }
    }
}

