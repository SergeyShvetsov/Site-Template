using System.Web;
 
namespace AppResources 
{
	public class Messages 
	{	
		private static global::System.Resources.ResourceManager resourceMan;
        
		private static global::System.Globalization.CultureInfo resourceCulture;
        
		internal Messages() {
		}
        
		public static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AppResources.Messages", typeof(Messages).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		public static string GetResourceString(string key) => ResourceManager.GetString(key, resourceCulture);
        
		public static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}
    
    
		[ResourceProperty]
    	public static string HomePage
        {
            get
			{
				return GetResourceString("HomePage");
			}
		}
		
		[ResourceProperty]
    	public static string PhoneList
        {
            get
			{
				return GetResourceString("PhoneList");
			}
		}
		}

	public class UseInClientScriptAttribute : System.Attribute { }
	public class ResourcePropertyAttribute : System.Attribute { }
}
