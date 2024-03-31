using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using MailApp.Services;
using Windows.UI.Xaml.Media;

namespace MailApp.Behaviors;

public class MailServicePersonPictrueBehavior : Behavior<Windows.UI.Xaml.Controls.PersonPicture>
{
    private bool _changedImage;
    private ImageSource _originImage;
    private CancellationTokenSource _cancellation;
    private IMailService _mailService;

    public IMailService MailService 
    {
        get => _mailService; 
        set
        {
            _mailService = value;
            StartImageUpdating();
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        StartImageUpdating();
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (_changedImage)
            AssociatedObject.ProfilePicture = _originImage;
    }


    void StartImageUpdating()
    {
        if (AssociatedObject == null)
            return;
        if (MailService is null)
            return;

        if (_cancellation != null)
            _cancellation.Cancel();

        _cancellation = new CancellationTokenSource();

        // run on dispatcher
        _ = RequestAndSetAvatar(_cancellation.Token);
    }

    async Task RequestAndSetAvatar(CancellationToken cancellationToken)
    {
        try
        {
            var avatar = await MailService.GetAvatarAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            // it might be null here,,,
            if (AssociatedObject == null)
                return;

            _originImage = AssociatedObject.ProfilePicture;
            _changedImage = true;

            AssociatedObject.ProfilePicture = avatar;
        }
        catch (OperationCanceledException)
        {
            // do nothing
        }
        catch (Exception)
        {
            // TODO: log something here
        }
    }
}
