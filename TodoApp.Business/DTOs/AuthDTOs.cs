namespace TodoApp.Business.DTOs;

public record LoginDto(string Username, string Password);
public record RegisterDto(string Username, string Email, string Password);
public record AuthResultDto(string Token, string Username, string Email);
