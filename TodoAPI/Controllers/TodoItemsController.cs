using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Route("api/TodoItems")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly TodoContext _context;
    private readonly ILogger<TodoItemsController> _logger;

    public TodoItemsController(TodoContext context, ILogger<TodoItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
    {
        _logger.LogDebug("Get all tasks");
        return await _context.TodoItems.Select(x => ItemToDto(x)).ToListAsync();
    }

    // GET: api/TodoItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(int id)
    {
        _logger.LogDebug($"Get task #{id}");
        var todoItem = await _context.TodoItems.FindAsync(id);


        if (todoItem == null)
        {
            _logger.LogError($"Task #{id} not found");
            return NotFound();
        }

        return ItemToDto(todoItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(int id, TodoItemDTO todoItemDto)
    {
        if (id != todoItemDto.Id)
        {
            _logger.LogError("Task #{Id} and data sent are not corresponding", id);
            return BadRequest();
        }

        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            _logger.LogError("Task #{Id} not found", id);
            return NotFound();
        }

        todoItem.Name = todoItemDto.Name;
        todoItem.IsComplete = todoItemDto.IsComplete;

        _context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Task #{Id} updated", id);
        }
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();

        }

        return NoContent();
    }

    // POST: api/TodoItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
    {

        var todoItem = new TodoItem
        {
            Name = todoItemDTO.Name,
            IsComplete = todoItemDTO.IsComplete
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task #{TodoItemId} added", todoItem.Id);

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDto(todoItem));
    }

    // DELETE: api/TodoItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            _logger.LogError("Task #{Id} not found", id);
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task #{Id} delete", id);

        return NoContent();
    }

    private bool TodoItemExists(int id)
    {
        return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();

    }

    private static TodoItemDTO ItemToDto(TodoItem todoItem) =>
        new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };
}