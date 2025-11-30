namespace Application.Exceptions;

public class BadRequestException(string ex) : Exception(ex) { }

public class AlreadyExistsException(string ex) : Exception(ex) { }

public class NotFoundException(string ex) : Exception(ex) { }