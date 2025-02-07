using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.AspNetCore.Mvc;

namespace DependencyCrawler.CSharpCodeAnalysis.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CSharpCodeAnalysisController(IDataCoreDTOFactory dataCoreDTOFactory) : ControllerBase
{
	[HttpGet(Name = "GetDataCoreDTO")]
	public DataCoreDTO GetDataCoreDTO()
	{
		return dataCoreDTOFactory.CreateDataCoreDTO();
	}
}