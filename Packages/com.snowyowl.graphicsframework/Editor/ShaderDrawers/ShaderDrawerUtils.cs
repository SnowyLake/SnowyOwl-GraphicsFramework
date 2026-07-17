using System;
using System.Linq;
using System.Text;
using UnityEditor;
using DrawerParameters = Needle.ShaderGraphMarkdown.MarkdownMaterialPropertyDrawer.DrawerParameters;

namespace SnowyOwl.GraphicsFramework.Editor
{
    public static class ShaderDrawerUtils
    {
        public static string GetDisyplayName(in DrawerParameters parameters, string prefix = "", int offset = 0)
        {
            var sb = new StringBuilder(prefix);
            var count = parameters.Count;
            for (int i = offset; i < count; i++)
            {
                sb.Append(parameters.Get(i, string.Empty));
                if (i != count - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
        
        public static MaterialProperty GetSelfProperty(in DrawerParameters parameters, MaterialProperty[] properties, string prefix = "")
        {
            var displayName = GetDisyplayName(parameters, prefix, 0);
            return properties.First(x => x.displayName.Equals(displayName, StringComparison.Ordinal));
        }
    }
}