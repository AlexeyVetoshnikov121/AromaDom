using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace AromaDom
{
    public class ProductsForm : Form
    {
        private readonly string _connectionString;
        private readonly int _materialId;
        private readonly string _materialName;
        private FlowLayoutPanel flowPanel;
        private Button btnBack;

        public ProductsForm(string connectionString, int materialId, string materialName)
        {
            _connectionString = connectionString;
            _materialId = materialId;
            _materialName = materialName;

            this.Text = $"Продукция с материалом: {_materialName}";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Georgia", 10f);

            btnBack = new Button
            {
                Text = "Назад",
                Size = new Size(120, 40),
                Location = new Point(20, 15),
                BackColor = Color.FromArgb(138, 90, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);

            Label title = new Label
            {
                Text = $"Продукция, использующая: {_materialName}",
                Font = new Font("Georgia", 12, FontStyle.Bold),
                Location = new Point(160, 22),
                AutoSize = true
            };
            this.Controls.Add(title);

            flowPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 70),
                Size = new Size(640, 420),
                AutoScroll = true,
                BackColor = Color.FromArgb(244, 230, 212)
            };
            this.Controls.Add(flowPanel);

            this.Load += ProductsForm_Load;
        }

        private void ProductsForm_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            flowPanel.Controls.Clear();

            try
            {
                var list = GetProductsForMaterial(_materialId);

                if (list.Count == 0)
                {
                    Label empty = new Label
                    {
                        Text = "Этот материал пока не используется ни в одной продукции.",
                        AutoSize = true,
                        Margin = new Padding(15)
                    };
                    flowPanel.Controls.Add(empty);
                    return;
                }

                foreach (var item in list)
                {
                    flowPanel.Controls.Add(CreateProductCard(item.Name, item.Quantity));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки продукции:\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<(string Name, decimal Quantity)> GetProductsForMaterial(int materialId)
        {
            var result = new List<(string, decimal)>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT p.Name, pm.Quantity
                    FROM ProductMaterials pm
                    INNER JOIN Products p ON pm.ProductId = p.Id
                    WHERE pm.MaterialId = @MaterialId
                    ORDER BY p.Name";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaterialId", materialId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add((reader.GetString(0), reader.GetDecimal(1)));
                        }
                    }
                }
            }
            return result;
        }

        private Panel CreateProductCard(string productName, decimal quantity)
        {
            Panel card = new Panel
            {
                Width = 600,
                Height = 70,
                BackColor = Color.White,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label nameLabel = new Label
            {
                Text = productName,
                Font = new Font("Georgia", 11, FontStyle.Bold),
                Location = new Point(15, 12),
                AutoSize = true
            };

            Label qtyLabel = new Label
            {
                Text = $"Необходимое количество материала: {quantity:F2}",
                Font = new Font("Georgia", 10),
                Location = new Point(15, 38),
                AutoSize = true,
                ForeColor = Color.FromArgb(138, 90, 68)
            };

            card.Controls.Add(nameLabel);
            card.Controls.Add(qtyLabel);
            return card;
        }
    }
}