using Bank.Domain.Exceptions;

namespace Bank.Domain.Exceptions;

public class NotFoundException(string msg) : DomainException(msg) { }