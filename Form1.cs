using ManageMaterials.wpf.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace AromaDom
{
    public class Form1 : Form
    {
        private MaterialRepository _repository;
        private FlowLayoutPanel flowLayoutPanelMaterials;
        private Button btnAddMaterial;

        public Form1()
        {

            this.Text = "Арома Дом — Материалы";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Georgia", 10f);

            btnAddMaterial = new Button();
            btnAddMaterial.Text = "Добавить материал";
            btnAddMaterial.Size = new Size(180, 40);
            btnAddMaterial.Location = new Point(20, 15);
            btnAddMaterial.BackColor = Color.FromArgb(138, 90, 68);
            btnAddMaterial.ForeColor = Color.White;
            btnAddMaterial.FlatStyle = FlatStyle.Flat;
            btnAddMaterial.Font = new Font("Georgia", 10f, FontStyle.Bold);
            btnAddMaterial.Click += BtnAddMaterial_Click;
            this.Controls.Add(btnAddMaterial);

            flowLayoutPanelMaterials = new FlowLayoutPanel();
            flowLayoutPanelMaterials.Location = new Point(20, 70);
            flowLayoutPanelMaterials.Size = new Size(840, 520);
            flowLayoutPanelMaterials.AutoScroll = true;
            flowLayoutPanelMaterials.BackColor = Color.White;
            flowLayoutPanelMaterials.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(flowLayoutPanelMaterials);

            string connectionString = ConfigurationManager.ConnectionStrings["AromaDomConnection"]?.ConnectionString
                ?? @"Data Source=localhost;Initial Catalog=AromaDom;Integrated Security=True;TrustServerCertificate=True";

            _repository = new MaterialRepository(connectionString);

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            flowLayoutPanelMaterials.Controls.Clear();

            try
            {
                List<Material> materials = _repository.GetAllMaterials();

                foreach (var material in materials)
                {
                    flowLayoutPanelMaterials.Controls.Add(CreateMaterialCard(material));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить список материалов.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateMaterialCard(Material material)
        {

            Panel card = new Panel
            {
                Width = 340,
                Height = 170,
                BackColor = Color.FromArgb(244, 230, 212),
                Margin = new Padding(12),
                Cursor = Cursors.Hand,
                Tag = material
            };

            Label title = new Label
            {
                Text = $"{material.TypeName} | {material.Name}",
                Font = new Font("Georgia", 11f, FontStyle.Bold),
                Location = new Point(12, 12),
                AutoSize = true,
                MaximumSize = new Size(310, 0)
            };

            Label minQty = new Label
            {
                Text = $"Минимальное количество: {material.MinQuantity} {material.Unit}",
                Font = new Font("Georgia", 9f),
                Location = new Point(12, 45),
                AutoSize = true
            };

            Label stock = new Label
            {
                Text = $"Количество на складе: {material.QuantityInStock} {material.Unit}",
                Font = new Font("Georgia", 9f),
                Location = new Point(12, 68),
                AutoSize = true
            };

            Label price = new Label
            {
                Text = $"Цена: {material.Price:F2} р / Единица измерения: {material.Unit}",
                Font = new Font("Georgia", 9f),
                Location = new Point(12, 91),
                AutoSize = true
            };

            Label required = new Label
            {
                Text = $"Требуемое количество: {material.RequiredQuantity:F2} {material.Unit}",
                Font = new Font("Georgia", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(138, 90, 68),
                Location = new Point(12, 120),
                AutoSize = true
            };

            card.Controls.Add(title);
            card.Controls.Add(minQty);
            card.Controls.Add(stock);
            card.Controls.Add(price);
            card.Controls.Add(required);

            Button btnProducts = new Button
            {
                Text = "Продукция",
                Size = new Size(100, 28),
                Location = new Point(15, 130),
                BackColor = Color.FromArgb(138, 90, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 8),

            };

            btnProducts.Click += (s, e) =>
            {
                using (var form = new ProductsForm(
                    @"Data Source=DESKTOP-LH87B9M\MSSQLSERVER100;Initial Catalog=AromaDom;Integrated Security=True;TrustServerCertificate=True",
                    material.Id,
                    material.Name))
                {
                    form.ShowDialog();
                }
            };

            card.Controls.Add(btnProducts);

            EventHandler clickHandler = (s, e) => OpenEditForm(material);
            card.Click += clickHandler;
            foreach (Control c in card.Controls)
                if (c != btnProducts)          
                    c.Click += clickHandler;

            return card;
        }

        private void OpenEditForm(Material material)
        {
            using (MaterialEditForm form = new MaterialEditForm(_repository, material))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadMaterials();
            }
        }

        private void BtnAddMaterial_Click(object sender, EventArgs e)
        {
            using (MaterialEditForm form = new MaterialEditForm(_repository, null))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadMaterials();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(707, 522);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }


    }
}