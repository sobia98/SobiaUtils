namespace Sobia.SigmaboyProject
{
    public static class GameConstants
    {
        // -------------------- Core Gameplay --------------------
        public const float CATCH_PROBABILITY_CAP = 0.9f;

        public const float MIN_GLOBAL_SPEED_MULTIPLIER = 0.1f;

        public const int FINISHED_GAME_HEARTS = 100000;

        // Session / stages
        public const int MAX_STAGES = 3;

        public const float SHUFFLE_WAIT_SECONDS = 1.0f;

        // -------------------- UI --------------------
        public const int TOTAL_HEARTS_FONT_SIZE = 36;

        public static readonly UnityEngine.Color TOTAL_HEARTS_COLOR = new UnityEngine.Color(1f, 0.1f, 0.1f);

        public const float POPUP_WORLD_Y_OFFSET = 1.5f;
        public const int POPUP_FONT_SIZE = 10;
        public const int POPUP_SORTING_ORDER = 100;

        // Popup animation
        public const float NO_TEXT_DURATION_SECONDS = 2.0f;

        public const float NO_TEXT_VERTICAL_OFFSET = 1.0f;
        public const float NO_TEXT_RISE_UNITS = 2.0f;

        // Heart popup
        public const float HEART_POPUP_DURATION_SECONDS = 2.0f;

        public const float HEART_POPUP_RISE_UNITS = 2.0f;
        public const float HEART_POP_SOUND_VOLUME = 0.7f;
        public const float HEART_POP_PITCH_MIN = 0.8f;
        public const float HEART_POP_PITCH_MAX = 1.2f;
        public const float DEFAULT_AUDIO_PITCH = 1f;
        public static readonly UnityEngine.Color HEART_POPUP_COLOR = new UnityEngine.Color(1f, 0.2f, 0.5f);

        // Per-girl heart popup pacing
        public const float HEART_POP_DELAY_NUMERATOR = 1.0f;

        public const float HEART_POP_DELAY_MIN = 0.01f;
        public const float HEART_POP_DELAY_MAX = 0.1f;

        // -------------------- Girl Rejection / Elimination --------------------
        public const float REJECT_NO_TEXT_PAUSE_SECONDS = 0.3f;

        public const float ELIMINATE_NO_TEXT_PAUSE_SECONDS = 0.1f;

        public const float RUN_AWAY_ANGLE_MAX_DEGREES = 360f;
        public const float RUN_AWAY_POINT_DISTANCE = 10f;
        public const float RUN_DURATION_SECONDS = 2.5f;
        public const float RUN_SPEED_MULTIPLIER = 4f;
        public const float FADE_DURATION_SECONDS = 0.5f;
        public const float DIRECTION_TOO_CLOSE_EPSILON = 0.1f;

        // Ugly
        public const int UGLY_HEART = 1;

        public const float UGLY_CHANCE = 0.45f;
        public const float UGLY_CHANCE_MAX = 0.75f;
        public const float UGLY_SPEED = 2.0f;
        public const float UGLY_SPEED_MAX = 2.0f;
        public const string UGLY_COLOR = "#3b1e30";

        // Normal
        public const int NORMAL_HEART = 5;

        public const float NORMAL_CHANCE = 0.60f;
        public const float NORMAL_SPEED = 3.0f;
        public const string NORMAL_COLOR = "#FFFFFF";

        // Average
        public const int AVERAGE_HEART = 15;

        public const float AVERAGE_CHANCE = 0.30f;
        public const float AVERAGE_CHANCE_MAX = 0.60f;
        public const float AVERAGE_SPEED = 4.0f;
        public const string AVERAGE_COLOR = "#c12a5e";

        // Hot
        public const int HOT_HEART = 50;

        public const float HOT_CHANCE = 0.30f;
        public const float HOT_SPEED = 5.0f;
        public const string HOT_COLOR = "#00FFFF";

        // Baddie
        public const int BADDIE_HEART = 200;

        public const float BADDIE_CHANCE = 0.05f;
        public const float BADDIE_CHANCE_MAX = 0.35f;
        public const float BADDIE_SPEED = 6.0f;
        public const string BADDIE_COLOR = "#f6a1b2";
    }
}