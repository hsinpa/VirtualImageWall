
public class General {
	public const int unitLayer = 9;
	public const int unitLayerMask = 1 << 9;

	public const int vehicleLayer = 10;
	public const int vehicleLayerMask = 1 << 10;
}

public class PoolingID {
	public const string ImageCard = "pooling@imagecard";
}



public class EventFlag {

    public class Game
    {
        public const string SetUp = "game.setup@event";
    }

    public class Scrollview {

        public const string OnScrollView = "scollview.target@event";

        public const string OnModalOpen = "modal.open@event";
        public const string OnModalClose = "modal.close@event";
    }
}
