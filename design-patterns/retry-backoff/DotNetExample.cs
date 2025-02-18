using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{

    // Features:
    // Exponential backoff: Waits longer between each retry.
    // Max retries: Stops after a set number of attempts.
    // Randomized jitter: Adds randomness to avoid overloading the server.

    // How It Works:
    // Tries to make an HTTP request.
    // If it fails, it retries up to maxRetries times.
    // Waits with exponential backoff before retrying (2^retryCount * 100ms + jitter).
    // Stops after max retries or when the request succeeds.
    // You can test this by using an invalid URL or temporarily blocking requests. 


    static async Task Main()
    {
        string url = "https://example.com/api"; // Replace with a real endpoint
        int maxRetries = 5;
        await ExecuteWithRetryAsync(() => MakeHttpRequestAsync(url), maxRetries);
    }

    static async Task ExecuteWithRetryAsync(Func<Task> operation, int maxRetries)
    {
        int retryCount = 0;
        Random jitter = new Random();
        
        while (retryCount < maxRetries)
        {
            try
            {
                await operation();
                return;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Attempt {retryCount} failed: {ex.Message}");
                
                if (retryCount == maxRetries)
                {
                    Console.WriteLine("Max retry attempts reached. Aborting.");
                    return;
                }
                
                int delay = (int)(Math.Pow(2, retryCount) * 100) + jitter.Next(0, 100);
                Console.WriteLine($"Retrying in {delay}ms...");
                await Task.Delay(delay);
            }
        }
    }

    static async Task MakeHttpRequestAsync(string url)
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"HTTP request failed with status {response.StatusCode}");
        }
        
        Console.WriteLine("Request succeeded!");
    }
}
