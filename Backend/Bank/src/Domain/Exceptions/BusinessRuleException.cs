using Bank.Domain.Exceptions;

namespace Bank.Domain.Exceptions;

public class BusinessRuleException(string msg) : DomainException(msg) { }