namespace RentACar.Tests.Constants
{
    public static class TestConstants
    {
        public static class Car
        {
            public const string TestBrand = "TestBrand";
            public const string TestModel = "TestModel";
            public const int TestYear = 2023;
            public const decimal TestPrice = 100.00m;
        }

        public static class User
        {
            public const string TestEmail = "test@example.com";
            public const string TestPassword = "TestPassword123!";
            public const string TestFirstName = "Test";
            public const string TestLastName = "User";
        }

        public static class Rental
        {
            public const int TestDuration = 7;
            public const decimal TestTotalPrice = 700.00m;
        }
    }
} 