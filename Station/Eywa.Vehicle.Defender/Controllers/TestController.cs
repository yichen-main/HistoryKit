namespace Eywa.Vehicle.Defender.Controllers;
public class TestController : ApiController
{
    const string excelFilesPath = "excel-files";

    [HttpGet(excelFilesPath)]
    public IActionResult Get()
    {
        try
        {
            var moduleName = DurableSetup.Link(HumanResourcesFlag.HumanPositionNameIsRequired);

            return Ok();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}