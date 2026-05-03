using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Business.DTOs;
using TodoApp.Business.Interfaces;
using TodoApp.Common;

namespace TodoApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ICurrentUserService _currentUserService;

    public TodoController(ITodoService todoService, ICurrentUserService currentUserService)
    {
        _todoService = todoService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _todoService.GetAllForUserAsync(_currentUserService.UserId);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _todoService.GetByIdForUserAsync(id, _currentUserService.UserId);
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoItemDto dto)
    {
        var created = await _todoService.CreateAsync(dto, _currentUserService.UserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTodoItemDto dto)
    {
        await _todoService.UpdateAsync(id, dto, _currentUserService.UserId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _todoService.DeleteAsync(id, _currentUserService.UserId);
        return NoContent();
    }
}
