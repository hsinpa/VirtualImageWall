
public class General {
	public const int unitLayer = 9;
	public const int unitLayerMask = 1 << 9;

	public const int vehicleLayer = 10;
	public const int vehicleLayerMask = 1 << 10;
}

public class PoolingID {
	public const string ImageCard = "pooling@imagecard";
}

public class InputEvent {
    //Draw
    public const UnityEngine.KeyCode DrawAll = UnityEngine.KeyCode.Alpha1;
    public const UnityEngine.KeyCode DrawLeft = UnityEngine.KeyCode.Alpha2;
    public const UnityEngine.KeyCode DrawRight = UnityEngine.KeyCode.Alpha3;
    public const UnityEngine.KeyCode ChangeCompanyLeft = UnityEngine.KeyCode.LeftArrow;
    public const UnityEngine.KeyCode ChangeCompanyRight = UnityEngine.KeyCode.RightArrow;

    //General
    public const UnityEngine.KeyCode SwitchMode = UnityEngine.KeyCode.Space;
    public const UnityEngine.KeyCode StagnateScene = UnityEngine.KeyCode.Alpha0;
    public const UnityEngine.KeyCode StaticBG = UnityEngine.KeyCode.Alpha9;
}


public class EventFlag {
    public class Animation {
        public const string Blink = "Blink";
        public const string Reset = "Reset";
        public const string Stagnate = "Stagnate";
    }

    public class Game
    {
        public const string SetUp = "game.setup@event";
    }

    public class Scrollview {

        public const string OnScrollView = "scollview.target@event";

        public const string OnModalOpen = "modal.open@event";
        public const string OnModalClose = "modal.close@event";
    }

    public class CompanyMap {
        public const string C1_ID = "C1";
        public const string C2_ID = "C2";
        public const string C3_ID = "C3";

        public const string C1_Fullname= "惠氏";
        public const string C2_Fullname = "卡多摩";
        public const string C3_Fullname = "大樹";
    }
}