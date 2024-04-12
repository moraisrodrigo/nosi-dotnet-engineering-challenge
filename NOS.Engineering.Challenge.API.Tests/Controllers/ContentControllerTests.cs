using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.API.Tests.Mock;
using NOS.Engineering.Challenge.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace NOS.Engineering.Challenge.API.Tests.Controllers;

public class ContentControllerTests
{
  [Fact]
  public async Task SearchContents_ReturnsOkResult()
  {
    Data.mockManager.Setup(manager => manager.GetManyContents()).ReturnsAsync(Data.mockContents);

    ContentController controller = new(Data.mockManager.Object, Data.logger.Object, Data.cacheService.Object);

    IActionResult result = await controller.SearchContents();

    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);

    IEnumerable<Content> contents = Assert.IsAssignableFrom<IEnumerable<Content>>(okResult.Value);

    Assert.NotEmpty(contents);
  }

  [Fact]
  public async Task GetManyContents_ReturnsOkResult()
  {
    Data.mockManager.Setup(manager => manager.GetManyContents()).ReturnsAsync(Data.mockContents);

    ContentController controller = new(Data.mockManager.Object, Data.logger.Object, Data.cacheService.Object);

    IActionResult result = await controller.GetManyContents();

    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);

    IEnumerable<Content> contents = Assert.IsAssignableFrom<IEnumerable<Content>>(okResult.Value);

    Assert.NotEmpty(contents);
  }

  [Fact]
  public async Task CreateContent_ReturnsOkResult()
  {
    Data.mockManager.Setup(manager => manager.CreateContent(It.IsAny<ContentDto>())).ReturnsAsync(Data.mockContent);

    ContentController controller = new(Data.mockManager.Object, Data.logger.Object, Data.cacheService.Object);

    IActionResult result = await controller.CreateContent(Data.mockContentInput);

    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);

    Content content = Assert.IsAssignableFrom<Content>(okResult.Value);

    Assert.NotNull(content);
  }

  [Fact]
  public async Task DeleteContent_ReturnsOkResult()
  {
    Data.mockManager.Setup(manager => manager.DeleteContent(It.IsAny<Guid>())).ReturnsAsync(Data.customGuid);

    ContentController controller = new(Data.mockManager.Object, Data.logger.Object, Data.cacheService.Object);

    IActionResult result = await controller.DeleteContent(Data.customGuid);

    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);

    Guid guid = Assert.IsAssignableFrom<Guid>(okResult.Value);

  }

  [Fact]
  public async Task GetContent_ReturnsOkResult()
  {
    Data.mockManager.Setup(manager => manager.GetContent(It.IsAny<Guid>())).ReturnsAsync(Data.mockContent);

    ContentController controller = new(Data.mockManager.Object, Data.logger.Object, Data.cacheService.Object);

    IActionResult result = await controller.GetContent(Data.customGuid);

    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);

    Content content = Assert.IsAssignableFrom<Content>(okResult.Value);

    Assert.NotNull(content);
  }

  [Fact]
  public async Task UpdateContent_ReturnsOkResult()
  {
    Data.mockManager.Setup(manager => manager.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(Data.mockContent);

    ContentController controller = new(Data.mockManager.Object, Data.logger.Object, Data.cacheService.Object);

    IActionResult result = await controller.UpdateContent(Data.customGuid, Data.mockContentInput);

    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);

    Content content = Assert.IsAssignableFrom<Content>(okResult.Value);

    Assert.NotNull(content);
  }

  [Fact]
  public async Task SearchContents_ReturnsResultsWithFilteredContents()
  {
    Data.mockManager.Setup(manager => manager.GetManyContents()).ReturnsAsync(Data.mockContents);

    ContentController controller = new(Data.mockManager.Object, Data.logger.Object, Data.cacheService.Object);

    IActionResult result = await controller.SearchContents("Sample Content 1", "Genre1");

    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
    IEnumerable<Content> contents = Assert.IsAssignableFrom<IEnumerable<Content>>(okResult.Value);

    Assert.NotEmpty(contents);
    Assert.Single(contents);
    Assert.Equal("Sample Content 1", contents.First().Title);
    Assert.Contains("Genre1", contents.First().GenreList);

    Data.mockManager.Verify(manager => manager.GetManyContents(), Times.Once);
  }


}
