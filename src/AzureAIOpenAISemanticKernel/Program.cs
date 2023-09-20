using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace AzureAIOpenAISemanticKernel;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var kernel = Kernel.Builder
            .WithAzureChatCompletionService(
                config.GetSection("DeploymentName").Value,
                config.GetSection("Endpoint").Value,
                config.GetSection("apiKey").Value
            )
            .Build();

        //For example:  I want a c# program for an user login with active directory

        // Sample 1
        Console.Write("Enter requirements: ");
        var requirements = Console.ReadLine();
        var prompt = $"""
            Write a program with the following requirements:
            {requirements}
            """;

        var promptConfig = new PromptTemplateConfig
        {
            Completion =
            {
                MaxTokens = 1000, Temperature = 0.2, TopP = 0.5
            }
        };

        var promptTemplate = new PromptTemplate(
            prompt, promptConfig, kernel
        );

        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

        var function = kernel.RegisterSemanticFunction("WriteMyCode", functionConfig);

        var outputCode = await kernel.RunAsync(function);

        Console.WriteLine(outputCode);

        // Sample 2
        //Console.Write("Enter requirements: ");
        //string? requirements = Console.ReadLine();

        //var plugin = kernel.ImportSemanticSkillFromDirectory("Plugins", "WriteMyCode");

        //var variables = new ContextVariables();
        //variables.Set("requirements", requirements);

        //var stepOne = await kernel.RunAsync(variables, plugin["Step1"]);

        //Console.WriteLine(stepOne);

    }
}