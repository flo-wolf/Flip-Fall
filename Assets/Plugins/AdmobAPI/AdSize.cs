namespace admob {
    public class AdSize {
        private int width;
        private int height;

        public static readonly AdSize Banner = new AdSize(320, 50);
        public static readonly AdSize MediumRectangle = new AdSize(300, 250);
        public static readonly AdSize IABBanner = new AdSize(468, 60);
        public static readonly AdSize Leaderboard = new AdSize(728, 90);
        public static readonly AdSize WideSkyscraper = new AdSize(120,600);
        public static readonly AdSize SmartBanner = new AdSize(-1,-2);

        public AdSize(int width, int height) {
            this.width = width;
            this.height = height;
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }
    }
}
