using MyUtilities;


public class StairsManager : UpgradablePropsManager
{
    protected override void Awake()
    {
        base.Awake(); 
    
        ServiceLocator.Register(this);
    }
}




