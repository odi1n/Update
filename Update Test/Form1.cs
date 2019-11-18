using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdateProject;

namespace Update_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Update_Forms upForm = new Update_Forms("");//указать ссылку на хостинг
            if (upForm.CheckUpdateProj() )//Проверяем есть ли новое обновление
                upForm.ShowDialog();//Открывает форму
        }
    }
}
