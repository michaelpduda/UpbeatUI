/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UpbeatUI.View.Converters
{
    public class MappingsToSelectorConverter : ValueConverterMarkupExtension<MappingsToSelectorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => new ControlMappingSelector(value as IDictionary<Type, Type>);

        private class ControlMappingSelector : DataTemplateSelector
        {
            private IDictionary<Type, Type> _mappings;

            public ControlMappingSelector(IDictionary<Type, Type> mappings) =>
                _mappings = mappings != null ? mappings : new Dictionary<Type, Type>();

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                var contextType = item?.GetType() ?? typeof(object);
                var controlType
                    = _mappings.ContainsKey(contextType)
                      && typeof(FrameworkElement).IsAssignableFrom(_mappings[contextType])
                        ? _mappings[contextType]
                    : null;
                return
                    controlType != null ? (DataTemplate)XamlReader.Parse(
                        $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:ns=\"clr-namespace:{controlType.Namespace};assembly={controlType.Assembly.FullName}\"><ns:{controlType.Name} /></DataTemplate>")
                    : (DataTemplate)XamlReader.Parse(
                        $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Label Content=\"{contextType.GetType().Name}\" /></DataTemplate>");
            }
        }
    }
}