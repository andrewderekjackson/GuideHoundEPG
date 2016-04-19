using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Core.UI.Controls {

    /// <summary>
    /// List of supported loader animations.
    /// </summary>
    public enum LoadAnimationType {
        IndicatorSmall,
        IndicatorBig,
        Arrows
    }

    /// <summary>
    /// Shows a spinning loader animation.
    /// Get new loader images from http://www.ajaxload.info/ 
    /// </summary>
    public class LoaderAnimation : Control {
        static LoaderAnimation() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoaderAnimation), new FrameworkPropertyMetadata(typeof(LoaderAnimation)));
        }

        public LoadAnimationType AnimationType {
            get { return (LoadAnimationType)GetValue(AnimationTypeProperty); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        public static readonly DependencyProperty AnimationTypeProperty = DependencyProperty.Register(
            "AnimationType", 
            typeof(LoadAnimationType), 
            typeof(LoaderAnimation), 
            new UIPropertyMetadata(LoadAnimationType.IndicatorBig));

    }
}
