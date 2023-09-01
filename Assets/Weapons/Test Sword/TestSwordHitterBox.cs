using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TestSwordHitterBox : AbstractMeleeHitterBox
{
    [SerializeField] GameObject spark;

    protected override void OnBlockerHit(Block data)
    {
        base.OnBlockerHit(data);

        Hitter.Attacking = false;
        Debug.Log("I hit a HitterBox, meaning I got blocked :(");
        GameObject s = Instantiate(spark);

        s.transform.position = data.point + data.normal * .05f;
        s.transform.Rotate(data.normal);

        Root.Debug.DrawLight(s.transform.position);
        Hitter.PostAttack();
    }
}
