using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.Managers;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Interfaces;
using NOS.Engineering.Challenge.API.Models;

namespace NOS.Engineering.Challenge.API.Tests.Mock;

public class Data
{
  public static readonly Guid customGuid = Guid.NewGuid();
  public static readonly Content mockContent = new(
    id: Guid.NewGuid(),
    title: "Sample Content 1",
    subTitle: "Sample Subtitle 1",
    description: "Sample Description 1",
    imageUrl: "sample-image-url-1",
    duration: 60,
    startTime: DateTime.UtcNow,
    endTime: DateTime.UtcNow.AddHours(1),
    genreList: new List<string> { "Genre1", "Genre2" }
  );

  // public static readonly ContentDto mockContentDto = new(
  //   title: "Sample Content 1",
  //   subTitle: "Sample Subtitle 1",
  //   description: "Sample Description 1",
  //   imageUrl: "sample-image-url-1",
  //   duration: 60,
  //   startTime: DateTime.UtcNow,
  //   endTime: DateTime.UtcNow.AddHours(1),
  //   genreList: new List<string> { "Genre1", "Genre2" }
  // );

  public static readonly ContentInput mockContentInput = new()
  {
    Title = "Sample Content 1",
    SubTitle = "Sample Subtitle 1",
    Description = "Sample Description 1",
    ImageUrl = "sample-image-url-1",
    Duration = 60,
    StartTime = DateTime.UtcNow,
    EndTime = DateTime.UtcNow.AddHours(1)
  };

  public static readonly List<Content?> mockContents = new()
    {
      new (
        id: customGuid,
        title: "Sample Content 1",
        subTitle: "Sample Subtitle 1",
        description: "Sample Description 1",
        imageUrl: "sample-image-url-1",
        duration: 60,
        startTime: DateTime.UtcNow,
        endTime: DateTime.UtcNow.AddHours(1),
        genreList: new List<string> { "Genre1", "Genre2" }
      ),
      new(
        id: Guid.NewGuid(),
        title: "Sample Content 2",
        subTitle: "Sample Subtitle 2",
        description: "Sample Description 2",
        imageUrl: "sample-image-url-2",
        duration: 90,
        startTime: DateTime.UtcNow,
        endTime: DateTime.UtcNow.AddHours(2),
        genreList: new List<string> { "Genre3", "Genre4" }
      )
    };

  public static readonly Mock<IContentsManager> mockManager = new();
  public static readonly Mock<ILogger<ContentController>> logger = new();
  public static readonly Mock<ICacheService<Content>> cacheService = new();

}
