namespace Hotel_SAAS_Backend.API.Models.Constants
{
    public static class Permissions
    {
        public static class Hotels
        {
            public const string Create = "hotels.create";
            public const string Read = "hotels.read";
            public const string Update = "hotels.update";
            public const string Delete = "hotels.delete";
        }

        public static class Rooms
        {
            public const string Create = "rooms.create";
            public const string Read = "rooms.read";
            public const string Update = "rooms.update";
            public const string Delete = "rooms.delete";
        }

        public static class Bookings
        {
            public const string Create = "bookings.create";
            public const string Read = "bookings.read";
            public const string Update = "bookings.update";
            public const string Delete = "bookings.delete";
            public const string CheckIn = "bookings.checkin";
            public const string CheckOut = "bookings.checkout";
        }

        public static class Users
        {
            public const string Create = "users.create";
            public const string Read = "users.read";
            public const string Update = "users.update";
            public const string Delete = "users.delete";
        }

        public static class Brands
        {
            public const string Create = "brands.create";
            public const string Read = "brands.read";
            public const string Update = "brands.update";
            public const string Delete = "brands.delete";
        }

        public static class Subscriptions
        {
            public const string Read = "subscriptions.read";
            public const string Update = "subscriptions.update";
        }

        public static class Dashboard
        {
            public const string View = "dashboard.view";
            public const string ViewAll = "dashboard.viewall";
        }

        public static class Reports
        {
            public const string Revenue = "reports.revenue";
            public const string Bookings = "reports.bookings";
            public const string Occupancy = "reports.occupancy";
            public const string Inventory = "reports.inventory";
            public const string Hotel = "reports.hotel";
            public const string Export = "reports.export";
        }
    }
}
