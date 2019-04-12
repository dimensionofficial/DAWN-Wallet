using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples {
	/// <summary>
	/// ChatLine component.
	/// </summary>
	public class ChatLineComponent : ListViewItem {
		// specify components for displaying item data
		[SerializeField]
		public Text UserName;
		
		[SerializeField]
		public Text Message;
		
		[SerializeField]
		public Text Time;

		/// <summary>
		/// Display ChatLine.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ChatLine item)
		{
			UserName.text = item.UserName;
			Message.text = item.Message;
			Time.text = item.Time.ToString("[HH:mm:ss]");

			// change colors depend of origin
			if (item.Type==ChatLineType.System)
			{
				UserName.color = Color.red;
				Message.color = Color.red;
			}
			else
			{
				UserName.color = Color.white;
				Message.color = Color.white;
			}

			Utilites.UpdateLayout(GetComponent<LayoutGroup>());
		}
	}
}