let notifyHud = null;

mp.events.add('showNotification', (text, iconpic) => {
    if(notifyHud == null)
    {
        notifyHud = mp.browsers.new("package://web/vue/index.html");
        notifyHud.execute(`gui.notify.showNotification('${text}', '${iconpic}');`)
    }
    else
    {
        notifyHud.execute(`gui.notify.showNotification('${text}', '${iconpic}');`)
    }
});