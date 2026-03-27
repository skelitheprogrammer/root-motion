using System;
using UnityEngine;
namespace Samples.Animation_Rigging._1._2._0.Animation_Rigging_Constraint_Samples.Info.Scripts
{
	public class Readme : ScriptableObject {
		public Texture2D icon;
		public string title;
		public Section[] sections;
		public bool loadedLayout;
	
		[Serializable]
		public class Section {
			public string heading, text, linkText, url;
		}
	}
}
