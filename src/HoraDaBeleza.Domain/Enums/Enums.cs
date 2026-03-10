namespace HoraDaBeleza.Domain.Enums;

public enum AppointmentStatus
{
    Pending      = 1,
    Confirmed    = 2,
    Cancelled    = 3,
    Completed    = 4,
    NoShow       = 5
}

public enum UserType
{
    Client       = 1,
    Professional = 2,
    Owner        = 3,
    Admin        = 4
}

public enum SubscriptionStatus
{
    Active    = 1,
    Cancelled = 2,
    Expired   = 3,
    Suspended = 4
}

public enum NotificationType
{
    AppointmentConfirmed = 1,
    AppointmentCancelled = 2,
    AppointmentReminder  = 3,
    NewReview            = 4,
    Promotion            = 5,
    System               = 6
}
