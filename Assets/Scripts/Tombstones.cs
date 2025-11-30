using UnityEngine;
using System.Collections;

public class Tombstones : MonoBehaviour, IInteractable
{
    public GhostController GhostController;
    public void Interact(PlayerController PlayerController)
    {
        TombstoneFeedback();
    }

    void TombstoneFeedback()
    {
        if (gameObject.name == "EvaTombstone") { FeedbackBanner.Instance.Show("Eva Ravenscroft... '1812-1860. Beloved wife, mother, friend'. How cliche and drab, obviously there's more to her..."); }
        if (gameObject.name == "TheodoreTombstone") { FeedbackBanner.Instance.Show("Theodore Ravenscroft... '1872-1880. Full of life and jubilant, lost too soon' How tragic... Wait, that's just this year. Curious..."); }
        if (gameObject.name == "EdwardTombstone") { FeedbackBanner.Instance.Show("Edward Ravenscroft... '1795-1870. Lived his life doing the same thing as when he died... Absolutely nothing.' Slothful... If only to have such a privileged life."); }
        if (gameObject.name == "???Tombstone") { FeedbackBanner.Instance.Show("I can't make out the name or date on this, this must have been here for a long while."); }
        if (gameObject.name == "BriarRoseTombstone") { FeedbackBanner.Instance.Show("Rose 'Briar Rose' Ravenscroft... '1848-1860. The whispers granted her their embrace first.' The whispers..?"); GhostController.tombstoneInteracted = true; }
        if (gameObject.name == "RosalinTombstone") { FeedbackBanner.Instance.Show("Rosalin Ravenscroft... '1848-1860. Couldn't bear to live without her beloved daughter.' Oh, poor woman..."); }
        if (gameObject.name == "RowanTombstone") { FeedbackBanner.Instance.Show("Rowan Ravenscroft... '1842-1861. Beloved father, loyal husband, grieving soul.' Imagine losing your whole family like that..."); }
        if (gameObject.name == "AlfredTombstone") { FeedbackBanner.Instance.Show("Alfred 'Big Al' Ravenscroft... '1780-1875 Appetite of a horse, stubbornness of a mule, ego of a king.' A vain glutton, I see."); }
    }
}
