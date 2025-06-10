using MyUtilities;

public class WCManager : UpgradablePropsManager
{
    protected override void Awake()
    {
        base.Awake(); 
    
        ServiceLocator.Register(this);
    }
}
