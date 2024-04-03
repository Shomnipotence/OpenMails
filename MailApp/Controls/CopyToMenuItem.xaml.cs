using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MailApp.Models;
using MailApp.Models.Events;
using MailApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace MailApp.Controls
{
    public sealed partial class CopyToMenuItem : UserControl
    {
        public CopyToMenuItem()
        {
            this.InitializeComponent();
        }


        public IMailService MailService
        {
            get { return (IMailService)GetValue(MailServiceProperty); }
            set { SetValue(MailServiceProperty, value); }
        }

        public IEnumerable<MailFolder> AllFolders
        {
            get { return (IEnumerable<MailFolder>)GetValue(AllFoldersProperty); }
            set { SetValue(AllFoldersProperty, value); }
        }

        public MailMessage Message
        {
            get { return (MailMessage)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public event EventHandler? ActionInvoked;
        public event EventHandler<MailMessageCopiedEventArgs>? MailMessageCopied;


        public static readonly DependencyProperty MailServiceProperty =
            DependencyProperty.Register(nameof(MailService), typeof(IMailService), typeof(CopyToMenuItem), new PropertyMetadata(null, UpdateControlMenus));

        public static readonly DependencyProperty AllFoldersProperty =
            DependencyProperty.Register(nameof(AllFolders), typeof(TreeNode<MailFolder>), typeof(CopyToMenuItem), new PropertyMetadata(null, UpdateControlMenus));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(MailMessage), typeof(CopyToMenuItem), new PropertyMetadata(null, UpdateControlMenus));


        private MenuFlyoutItem CreateMenuFlyoutSubItemOfMoveToCommand(MailFolder folderNode)
        {
            MenuFlyoutItem item = new MenuFlyoutItem()
            {
                Text = folderNode.Name,
                Command = new AsyncRelayCommand<MailMessage>(async message =>
                {
                    ActionInvoked?.Invoke(this, EventArgs.Empty);

                    if (message == null ||
                        MailService is null)
                        return;

                    try
                    {
                        var newMessage = await MailService.CopyMessageAsync(message, folderNode, default);

                        MailMessageCopied?.Invoke(this, new MailMessageCopiedEventArgs(message, newMessage));
                    }
                    catch
                    {
                        var strings = App.Host.Services.GetRequiredService<I18nStrings>();

                        App.Host.Services
                            .GetRequiredService<ToastService>()
                            .ToastError(strings.StringFailedToMove);
                    }
                })
            };

            item.SetBinding(MenuFlyoutItem.CommandParameterProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(Message))
            });

            return item;
        }

        private void UpdateMenus()
        {
            string kwd = searchBox.Text.Trim();
            menuContainer.Children.Clear();

            if (AllFolders is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(kwd))
            {
                foreach (var folder in AllFolders)
                {
                    var item = CreateMenuFlyoutSubItemOfMoveToCommand(folder);
                    menuContainer.Children.Add(item);
                }
            }
            else
            {
                foreach (var folder in AllFolders)
                {
                    if (!folder.Name.Contains(kwd, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var item = CreateMenuFlyoutSubItemOfMoveToCommand(folder);
                    menuContainer.Children.Add(item);
                }
            }
        }

        private static void UpdateControlMenus(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not CopyToMenuItem menuItem)
            {
                return;
            }

            menuItem.UpdateMenus();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMenus();
        }
    }
}
