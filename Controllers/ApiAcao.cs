using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vantage.Model;

namespace Vantage.Controllers
{
	[Route("[controller]")]
	public class ApiAcaoController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ApiAcaoController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet("PrecoAcaoDiario")]
		public async Task<IActionResult> TestAsync(string apiKeyAcesso)
		{
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync($"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=PETR4.SA&apikey={apiKeyAcesso}");
			var json = await response.Content.ReadAsStringAsync();

			//var openValue = data.TimeSeries?.FirstOrDefault().Value?.High;
			var data = JsonConvert.DeserializeObject<AlphaVantageData>(json);

			if (!string.IsNullOrWhiteSpace(json))
			{
				var dataResponse = JsonConvert.DeserializeObject<AlphaVantageData>(json);

				if (dataResponse != null && data.TimeSeries != null)
				{
					// Obtém a data atual (sem a hora) para comparar com as entradas da API
					var currentDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

					// Verifica se a data atual está presente nas entradas da API
					if (dataResponse.TimeSeries.ContainsKey(currentDate))
					{
						// Obtém os dados correspondentes à data atual
						var todayData = dataResponse.TimeSeries[currentDate];


						// Retorna os dados de hoje
						return Ok(new
						{
							Date = currentDate,
							Open = todayData.Open,
							High = todayData.High,
							Low = todayData.Low,
							Close = todayData.Close
						});
					}
					else
					{
						return NotFound("Não há dados disponíveis para a data atual na API.");
					}
				}
			}

			return BadRequest();

		}
		
		[HttpGet("PrecoAcaoMensal")]
		public async Task<IActionResult> TestAsync2(string apiKeyAcesso)
		{
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=ITUB4.SA&apikey={apiKeyAcesso}");
			var json = await response.Content.ReadAsStringAsync();

			//var openValue = data.TimeSeries?.FirstOrDefault().Value?.High;
			var dataReponse = JsonConvert.DeserializeObject<AlphaVantageData>(json);

			if (dataReponse != null)
			{
				return Ok(dataReponse);
			}

			return BadRequest();

		}
		
		
		
	//   "todayOpen": "32.7800",
 	//   "todayHigh": "33.0600",
 	//   "todayLow": "32.2400",
 	//  "todayClose": "32.6900"
		
		
	// "2024-02-01": {
	// 	   "open": "32.7800",
	//   "high": "33.0600",
	//   "low": "32.2400",
	//   "close": "32.6900",
	
	
	//  "2024-01-31": {
	//   "open": "32.4800",
	//   "high": "33.3300",
	//   "low": "32.4700",
	//   "close": "32.7800",
	}
}