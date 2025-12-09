using Azure.Identity;
using HCI.AIAssistant.API.Managers;
using HCI.AIAssistant.API.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => 
{
    options.AddPolicy(name: "CORS",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

var vaultName = builder
        .Configuration[$"AppConfigurations{ConfigurationPath.KeyDelimiter}KeyVaultName"];
var secretsPrefix = builder
        .Configuration[$"AppConfigurations{ConfigurationPath.KeyDelimiter}SecretsPrefix"];

if (string.IsNullOrWhiteSpace(vaultName))
{
    throw new ArgumentNullException("KeyVaultName", "Missing name for key vault.");
}

if (string.IsNullOrWhiteSpace(secretsPrefix))
{
    throw new ArgumentNullException("SecretsPrefix", "Missing secrets prefix.");
}

var vaultUri = new Uri($"https://{vaultName}.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(
    vaultUri,
    new DefaultAzureCredential(),
    new CustomSecretManager(secretsPrefix)
);

// configure values from the app settings
builder.Services.Configure<SecretsService>(builder.Configuration.GetSection("Secrets"));
builder.Services.Configure<AppConfigurationService>(builder.Configuration.GetSection("AppConfigurations"));

// add the services as singletons to be injectable in classes
builder.Services.AddSingleton<ISecretsService>(
    provider => provider.GetRequiredService<IOptions<SecretsService>>().Value
);
builder.Services.AddSingleton<IAppConfigurationsService>(
    provider => provider.GetRequiredService<IOptions<AppConfigurationService>>().Value
);

builder.Services.AddSingleton<IParametricFunctions, ParametricFunctions>();
builder.Services.AddSingleton<IAIAssistantService, AIAssistantService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("CORS");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// test if the secrets get read
Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.EndPoint);
Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.Key);
Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.Id);
Console.WriteLine(app.Services.GetService<ISecretsService>()?.IoTHubSecrets?.ConnectionString);
Console.WriteLine(app.Services.GetService<IAppConfigurationsService>()?.KeyVaultName);
Console.WriteLine(app.Services.GetService<IAppConfigurationsService>()?.SecretsPrefix);
Console.WriteLine(app.Services.GetService<IAppConfigurationsService>()?.IoTDeviceName);

app.Run();
