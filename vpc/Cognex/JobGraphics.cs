using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognex.VisionPro
{
    internal class JobGraphicsGeneral
    {
        internal void UpdateGraphics(RxInterface r, Cognex.VisionPro.Display.CogDisplay cogdisplay)
        {
            SwitchGraphicsVisibility(cogdisplay);
            List<object[]> li = null;
            if (r != null)
                li = r.ResultGraphics;
            if (li != null)
                for (int i = 0; i < li.Count; i++)
                {
                    if (li[i] != null && li[i].Length > 0)
                    {
                        AddToControls(li[i][0], cogdisplay);
                    }
                }
            if (r.img != null)
            {
                CogRectangle cr = new CogRectangle();
                cr.SetXYWidthHeight(0, 0, r.img.Width, r.img.Height);
                cr.SelectedSpaceName = "@";
                if (r.Passed)
                    cr.Color = CogColorConstants.Green;
                else
                    cr.Color = CogColorConstants.Red;
                cr.LineWidthInScreenPixels = 7;
                AddToControls(cr, cogdisplay);
            }
        }
        internal void UpdateGraphics(int index, ResultStruct rx, Cognex.VisionPro.Display.CogDisplay cogdisplay)
        {
            SwitchGraphicsVisibility(cogdisplay);
            List<object[]> li = null;
            R1Class r = rx[index] as R1Class;
            if (r != null)
                li = r.ResultGraphics;
            if (li != null)
                for (int i = 0; i < li.Count; i++)
                {
                    if (li[i] != null && li[i].Length > 0)
                    {
                        AddToControls(li[i][0], cogdisplay);
                    }
                }
            if (r.img != null)
            {
                CogRectangle cr = new CogRectangle();
                cr.SelectedSpaceName = "@";
                cr.SetXYWidthHeight(0, 0, r.img.Width, r.img.Height);
                if (r.Passed)
                    cr.Color = CogColorConstants.Green;
                else
                    cr.Color = CogColorConstants.Red;
                cr.LineWidthInScreenPixels = 7;
                AddToControls(cr, cogdisplay);
            }
        }
        internal JobGraphicsGeneral()
        {
            Init();
        }
        internal void Init()
        {
        }
        internal void SwitchGraphicsVisibility(Cognex.VisionPro.Display.CogDisplay cd)
        {
            if (cd != null)
            {
                cd.InteractiveGraphics.Clear();
                cd.StaticGraphics.Clear();
            }
        }
        internal void AddToControls(ICogGraphicInteractive ic, Cognex.VisionPro.Display.CogDisplay cogdisplay)
        {
            if (cogdisplay != null && ic != null)
            {
                ic.SelectedColor = ic.Color;
                cogdisplay.InteractiveGraphics.Add(ic, null, false);
            }
        }
        internal void AddToControls(object obj, Cognex.VisionPro.Display.CogDisplay cogdisplay)
        {
            ICogGraphicInteractive ic = obj as ICogGraphicInteractive;
            if (ic != null)
            {
                if (cogdisplay != null && ic != null)
                {
                    ic.GraphicDOFEnableBase = CogGraphicDOFConstants.None;
                    ic.Interactive = true;
                    ic.Visible = true;
                    ic.SelectedColor = ic.Color;
                    cogdisplay.InteractiveGraphics.Add(ic, null, false);
                }
                return;
            }
            CogGraphicInteractiveCollection cic = obj as CogGraphicInteractiveCollection;
            if (cic != null && cogdisplay != null)
            {
                foreach (object objc in cic)
                {
                    ic = objc as ICogGraphicInteractive;
                    if (ic != null)
                    {
                        ic.GraphicDOFEnableBase = CogGraphicDOFConstants.None;
                        ic.Interactive = true;
                        ic.SelectedColor = ic.Color;
                        ic.Visible = true;
                        cogdisplay.InteractiveGraphics.Add(ic, null, false);
                    }
                }
                return;
            }
            CogGraphicCollection cgc = obj as CogGraphicCollection;
            if (cgc != null && cogdisplay != null)
            {
                foreach (object objc in cgc)
                {
                    ICogGraphic icg = objc as ICogGraphic;
                    if (icg != null)
                    {
                        icg.Visible = true;
                        cogdisplay.StaticGraphics.Add(icg, null);
                    }
                }
                return;
            }
        }
    }
}
