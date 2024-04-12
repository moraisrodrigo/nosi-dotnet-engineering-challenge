using System.Net;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Interfaces;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;
    private readonly ICacheService<Content> _cacheService;

    public ContentController(IContentsManager manager, ILogger<ContentController> logger, ICacheService<Content> cacheService)
    {
        _manager = manager;
        _logger = logger;
        _cacheService = cacheService;
    }

    [HttpGet]
    [Obsolete("This endpoint is deprecated. Use GET /api/v1/Content/Search instead.")]
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

            foreach (Content? content in contents)
            {
                if (content != null) await _cacheService.Set(content.Id, content).ConfigureAwait(false);
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

    [HttpGet("Search")]
    public async Task<IActionResult> SearchContents(string? title = null, string? genre = null)
    {
        _logger.LogInformation("[{method}]: Attempting to SearchContents", nameof(SearchContents));
        try
        {
            IEnumerable<Content?> contents = await _manager.GetManyContents().ConfigureAwait(false);

            // Filter out null contents
            contents = contents.Where(content => content != null);

            if (!string.IsNullOrEmpty(title))
            {
                contents = contents.Where(content => content?.Title != null && content.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                contents = contents.Where(content => content?.GenreList != null && content.GenreList.Contains(genre));
            }

            foreach (Content? content in contents)
            {
                if (content != null) await _cacheService.Set(content.Id, content).ConfigureAwait(false);
            }

            _logger.LogInformation("[{method}]: SearchContents method executed successfully.", nameof(SearchContents));
            return Ok(contents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while searching contents.", nameof(SearchContents));
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching contents.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        _logger.LogInformation("[{method}]: Attempting to retrieve content with ID: '{id}'", nameof(GetContent), id);

        try
        {

            Content? content = await _cacheService.Get(id).ConfigureAwait(false);

            if (content != null)
            {
                _logger.LogInformation("[{method}]: Content retrieved successfully with ID: '{id}'.", nameof(GetContent), id);
                return Ok(content);
            }

            content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(GetContent), id);
                return NotFound();
            }

            await _cacheService.Set(content.Id, content).ConfigureAwait(false);

            _logger.LogInformation("[{method}]: Content retrieved successfully with ID: '{id}'", nameof(GetContent), id);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while getting content with ID: '{id}'.", nameof(GetContent), id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput contentInput
        )
    {
        _logger.LogInformation("[{method}]: Attempting to create a content", nameof(CreateContent));
        try
        {
            Content? content = await _manager.CreateContent(contentInput.ToDto()).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogInformation("[{method}]: Failed to create content.", nameof(CreateContent));
                return Problem();
            }

            await _cacheService.Set(content.Id, content).ConfigureAwait(false);

            _logger.LogInformation("[{method}]: Content created successfully.", nameof(CreateContent));
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{method}]: An error occurred while creating content.", nameof(CreateContent));
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation("[{method}]: Attempting to update content with ID: '{id}'", nameof(UpdateContent), id); ;

        try
        {
            Content? updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

            if (updatedContent == null)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(UpdateContent), id);
                return NotFound();
            }

            await _cacheService.Set(id, updatedContent).ConfigureAwait(false); ;

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
            await _cacheService.Remove(id);

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
            Content? content = await _cacheService.Get(id) ?? await _manager.GetContent(id).ConfigureAwait(false);

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

            if (updatedContent != null) await _cacheService.Set(id, updatedContent).ConfigureAwait(false);

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
            // Content? content = await _manager.GetContent(id).ConfigureAwait(false);
            Content? content = await _cacheService.Get(id) ?? await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogInformation("[{method}]: Content with ID: '{id}' not found.", nameof(AddGenres), id);
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

            if (updatedContent != null) await _cacheService.Set(id, updatedContent).ConfigureAwait(false);

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