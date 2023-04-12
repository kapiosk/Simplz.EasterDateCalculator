var builder = WebApplication.CreateBuilder(args);
await using var app = builder.Build();

app.MapGet("/{year:int?}", (int? year) =>
{
    year ??= DateTime.UtcNow.Year;
    return Results.Content(@$"
<h1>{year.Value}</h1>
<p>Catholic Easter: {CalculateCatholicEaster(year.Value):yyyy-MM-dd}</p>
<p>Passover Easter: {CalculateJewishPassover(year.Value):yyyy-MM-dd}</p>
<p>Orthodox Easter: {CalculateOrthodoxEaster(year.Value):yyyy-MM-dd}</p>
", "text/html");
});

await app.RunAsync();

//Pentecost Monday = Easter Sunday + 50

// const string Passover = @"
// The date of the Jewish holiday of Passover is calculated based on the Hebrew calendar, which is a lunisolar calendar system. This means that it is based on the cycles of the moon and the solar year, and it takes into account both lunar and solar phenomena.
// The Hebrew calendar has 12 lunar months in a year, with each month beginning at the first sighting of the new moon. However, a lunar month is only 29.5 days long, which means that the Hebrew calendar would fall behind the solar year over time if no adjustments were made. To account for this, the Hebrew calendar has a leap year every two to three years, which adds an extra month to the calendar to keep it in sync with the solar year.
// The date of Passover is determined based on the Hebrew calendar. It is celebrated on the 15th day of the Hebrew month of Nisan, which falls in the spring season in the Northern Hemisphere. The calculation of the date of Passover takes into account the phase of the moon and the position of the sun in relation to the equinox.
// Specifically, Passover begins on the evening of the 14th day of Nisan, which is the day of the full moon closest to the vernal equinox. If the full moon falls on a Tuesday, Thursday, or Saturday, the holiday is postponed by one day to avoid conflicting with the Jewish Sabbath. Additionally, Passover must not begin before the vernal equinox, which usually falls on March 20 or 21 in the Gregorian calendar.
// Overall, the date of Passover varies each year on the Gregorian calendar, but usually falls in late March or April.
// ";

// const string CatholicEaster = @"
// Calculating the date of Catholic Easter is based on the first full moon after the vernal (spring) equinox. The vernal equinox is the day when the length of the day and night are almost equal, and it usually falls on March 20 or 21.
// The Catholic Church has defined that Easter should fall on the Sunday following the first full moon after the vernal equinox. However, the ""full moon"" in this calculation is not the actual full moon, but a theoretical one known as the ""ecclesiastical full moon"". It is based on an approximation of the phases of the moon, which was established by a Church council in the 6th century.
// To calculate the date of Catholic Easter, a series of calculations based on this definition are performed using a formula known as the ""Meeus/Jones/Butcher algorithm"". This algorithm takes into account the year, the phases of the moon, and other factors to determine the date of Easter for a given year.
// The result is a date in March or April, depending on the year, which is considered the most important Christian holiday and is celebrated worldwide by Catholics and many other Christian denominations.
// ";

static DateTime CalculateJewishPassover(int year)
{
    int month = 1;
    int day = 15;
    int dayOfWeek = (int)new DateTime(year, month, day).DayOfWeek;
    int correction;

    // If the 15th of Nissan falls on a Sunday, Wednesday, or Friday, the day is corrected
    if (dayOfWeek == 0 || dayOfWeek == 3 || dayOfWeek == 5)
    {
        correction = 1;
    }
    else
    {
        correction = 0;
    }

    // Calculate the date of Passover
    month = 1;
    day = 15 + correction;
    if (new DateTime(year, month, day).DayOfWeek != DayOfWeek.Saturday)
    {
        month = 1;
        day = 14 + correction;
    }

    DateTime passoverDate = new DateTime(year, month, day);

    return passoverDate;
}

static DateTime CalculateCatholicEaster(int year)
{
    int a = year % 19;
    int b = year / 100;
    int c = year % 100;
    int d = b / 4;
    int e = b % 4;
    int f = (b + 8) / 25;
    int g = (b - f + 1) / 3;
    int h = ((19 * a) + b - d - g + 15) % 30;
    int i = c / 4;
    int k = c % 4;
    int L = (32 + (2 * e) + (2 * i) - h - k) % 7;
    int m = (a + (11 * h) + (22 * L)) / 451;
    int month = (h + L - (7 * m) + 114) / 31;
    int day = ((h + L - (7 * m) + 114) % 31) + 1;

    return new DateTime(year, month, day);
}

static DateTime CalculateOrthodoxEaster(int year)
{
    //Meeus's Julian algorithm
    //Orthodox Easter takes place between April 4 and May 8, following the first full moon after Passover
    int a = year % 4;
    int b = year % 7;
    int c = year % 19;
    int d = (19 * c + 15) % 30;
    int e = (2 * a + 4 * b - d + 34) % 7;
    int f = d + e + 114;
    int month = f / 31;
    int day = (f % 31) + 1;
    DateTime julianDate = new DateTime(year, month, day);
    return julianDate.AddDays(13); //Gregorian 
}