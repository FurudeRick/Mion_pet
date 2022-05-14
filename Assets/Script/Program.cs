using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using System;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Assets.Script
{
    public class Program : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            KeyboardHook.HookStart();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}