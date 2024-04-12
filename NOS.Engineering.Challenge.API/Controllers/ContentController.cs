using System.Net;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;

    public ContentController(IContentsManager manager, ILogger<ContentController> logger)
    {
        _manager = manager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        _logger.LogInformation("[{method}]: Attempting to retrieve contents...", nameof(GetManyContents));

        try
        {
            IEnumerable<Content?> contents = await _manager.GetManyContents().ConfigureAwait(false);

            if (!contents.Any())
            {
                _logger.LogInformation("[{method}]: No contents found.", nameof(GetManyContents));
                return NotFound();
            }

            _logger.LogInformation("[{method}]: Retrived {numberOfContents} contents.", nameof(GetManyContents), contents.Count());
            return Ok(contents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while getting contents.", nameof(GetManyContents));
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        _logger.LogInformation("[{method}]: Attempting to retrieve content with ID: '{id}'", nameof(GetContent), id);

        try
        {
            Content? content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(GetContent), id);
                return NotFound();
            }

            _logger.LogInformation("[{method}]: Content retrieved successfully with ID '{id}'", nameof(GetContent), id);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while getting content with ID '{id}'.", nameof(GetContent), id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation("[{method}]: Attempting to create a content", nameof(GetContent));
        try
        {
            Content? createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

            if (createdContent == null)
            {
                _logger.LogInformation("[{method}]: Failed to create content.", nameof(GetContent));
                return Problem();
            }

            _logger.LogInformation("[{method}]: Content created successfully.", nameof(GetContent));
            return Ok(createdContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while creating content.", nameof(GetContent));
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation("[{method}]: Attempting to update content with ID: '{id}'", nameof(GetContent), id); ;

        try
        {
            Content? updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

            if (updatedContent == null)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(UpdateContent), id);
                return NotFound();
            }

            _logger.LogInformation("[{method}]: Content updated successfully with ID '{id}'", nameof(UpdateContent), id);
            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while updating content with ID: '{id}'.", nameof(UpdateContent), id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        _logger.LogInformation("[{method}]: Attempting to delete content with ID: '{id}'", nameof(DeleteContent), id);

        try
        {
            Guid deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);

            if (deletedId == Guid.Empty)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(DeleteContent), id);
                return NotFound();
            }

            _logger.LogInformation("[{method}]: Content deleted successfully with ID '{id}'", nameof(DeleteContent), id);
            return Ok(deletedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while deleting content with ID {id}.", nameof(DeleteContent), id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genreList
    )
    {
        _logger.LogInformation("[{method}]: Attempting to add genres to content with ID: '{id}'", nameof(AddGenres), id);

        try
        {
            Content? content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(AddGenres), id);
                return NotFound();
            }

            // Create a HashSet of existing genres for efficient lookup
            HashSet<string> existingGenres = new(content.GenreList, StringComparer.OrdinalIgnoreCase);

            // Initialize lists for new genres and duplicate genres
            List<string> newGenres = new();
            List<string> duplicateGenres = new();

            foreach (string genre in genreList)
            {
                if (!existingGenres.Contains(genre))
                    newGenres.Add(genre);
                else
                    duplicateGenres.Add(genre);
            }

            //return bad request with the duplicates list
            if (duplicateGenres.Any())
            {
                string message = $"Genres: [{string.Join(", ", duplicateGenres.Select(genre => $"'{genre}'"))}] already exist";
                _logger.LogWarning("[{method}]: {message} in content with ID '{id}'", nameof(AddGenres), message, id);
                return BadRequest(new GenericMessage(message));
            }

            List<string> updatedGenres = content.GenreList.Concat(newGenres).ToList();

            ContentDto updatedContentDto = new(
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.StartTime,
                content.EndTime,
                updatedGenres
            );

            Content? updatedContent = await _manager.UpdateContent(id, updatedContentDto).ConfigureAwait(false);

            _logger.LogInformation("[{method}]: Genres added successfully to content with ID '{id}'", nameof(AddGenres), id);
            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while adding genres to content with ID '{id}'.", nameof(AddGenres), id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genresList
    )
    {
        _logger.LogInformation("[{method}]: Attempting to remove genres from content with ID: '{id}'", nameof(RemoveGenres), id);

        try
        {
            Content? content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(RemoveGenres), id);
                return NotFound();
            }

            HashSet<string> existingGenres = new(content.GenreList, StringComparer.OrdinalIgnoreCase);

            List<string> genresToRemove = new();

            foreach (string genre in genresList)
            {
                if (existingGenres.Contains(genre)) genresToRemove.Add(genre);
            }

            if (!genresToRemove.Any())
            {
                _logger.LogInformation("[{method}]: No genres to remove for content with ID: '{id}'", nameof(RemoveGenres), id);
                return BadRequest("No genres to remove.");
            }

            List<string> updatedGenres = content.GenreList.Except(genresToRemove, StringComparer.OrdinalIgnoreCase).ToList();

            ContentDto updatedContentDto = new(
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.StartTime,
                content.EndTime,
                updatedGenres
            );

            Content? updatedContent = await _manager.UpdateContent(id, updatedContentDto).ConfigureAwait(false);

            _logger.LogInformation("[{method}]: Genres removed successfully from content with ID: '{id}'", nameof(RemoveGenres), id);
            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while removing genres from content with ID: '{id}'", nameof(RemoveGenres), id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

}