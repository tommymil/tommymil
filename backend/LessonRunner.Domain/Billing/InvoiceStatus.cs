namespace LessonRunner.Domain.Billing;

public enum InvoiceStatus
{
    Draft = 0,
    Open = 1,
    Paid = 2,
    Overdue = 3,
    Cancelled = 4
}
