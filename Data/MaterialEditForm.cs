using ManageMaterials.wpf.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace AromaDom
{
    public partial class MaterialEditForm : Form
    {
        private readonly MaterialRepository _repository;
        private readonly Material _currentMaterial;
        private readonly bool _isEditMode;

        // Элементы управления
        private ComboBox cmbType;
        private TextBox txtName;
        private TextBox txtPrice;
        private TextBox txtUnit;
        private TextBox txtPack;
        private TextBox txtStock;
        private TextBox txtMinQuantity;
        private Button btnSave;
        private Button btnBack;

        public MaterialEditForm(MaterialRepository repository, Material material)
        {
            _repository = repository;
            _currentMaterial = material;
            _isEditMode = material != null;

            this.Text = _isEditMode ? "Редактирование материала" : "Добавление материала";
            this.Size = new Size(480, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Georgia", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
            this.Load += MaterialEditForm_Load;
        }

        private void CreateControls()
        {
            int leftLabel = 30;
            int leftInput = 200;
            int top = 30;
            int step = 45;


            AddLabel("Тип материала:", leftLabel, top);
            cmbType = new ComboBox();
            cmbType.Location = new Point(leftInput, top);
            cmbType.Size = new Size(220, 28);
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbType);
            top += step;

            AddLabel("Наименование:", leftLabel, top);
            txtName = new TextBox();
            txtName.Location = new Point(leftInput, top);
            txtName.Size = new Size(220, 28);
            this.Controls.Add(txtName);
            top += step;

            AddLabel("Цена:", leftLabel, top);
            txtPrice = new TextBox();
            txtPrice.Location = new Point(leftInput, top);
            txtPrice.Size = new Size(220, 28);
            this.Controls.Add(txtPrice);
            top += step;

            AddLabel("Единица измерения:", leftLabel, top);
            txtUnit = new TextBox();
            txtUnit.Location = new Point(leftInput, top);
            txtUnit.Size = new Size(220, 28);
            this.Controls.Add(txtUnit);
            top += step;

            AddLabel("Кол-во в упаковке:", leftLabel, top);
            txtPack = new TextBox();
            txtPack.Location = new Point(leftInput, top);
            txtPack.Size = new Size(220, 28);
            this.Controls.Add(txtPack);
            top += step;

            AddLabel("Количество на складе:", leftLabel, top);
            txtStock = new TextBox();
            txtStock.Location = new Point(leftInput, top);
            txtStock.Size = new Size(220, 28);
            this.Controls.Add(txtStock);
            top += step;

            AddLabel("Минимальное количество:", leftLabel, top);
            txtMinQuantity = new TextBox();
            txtMinQuantity.Location = new Point(leftInput, top);
            txtMinQuantity.Size = new Size(220, 28);
            this.Controls.Add(txtMinQuantity);
            top += step + 20;

            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Size = new Size(140, 40);
            btnSave.Location = new Point(80, top);
            btnSave.BackColor = Color.FromArgb(138, 90, 68);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnBack = new Button();
            btnBack.Text = "Назад";
            btnBack.Size = new Size(140, 40);
            btnBack.Location = new Point(250, top);
            btnBack.Click += BtnBack_Click;
            this.Controls.Add(btnBack);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(x, y + 4);
            lbl.AutoSize = true;
            this.Controls.Add(lbl);
        }

        private void MaterialEditForm_Load(object sender, EventArgs e)
        {
            LoadTypes();

            if (_isEditMode)
            {
                cmbType.SelectedValue = _currentMaterial.MaterialTypeId;
                txtName.Text = _currentMaterial.Name;
                txtPrice.Text = _currentMaterial.Price.ToString("F2");
                txtUnit.Text = _currentMaterial.Unit;
                txtStock.Text = _currentMaterial.QuantityInStock.ToString("F2");
                txtMinQuantity.Text = _currentMaterial.MinQuantity.ToString("F2");
                txtPack.Text = _currentMaterial.QuantityInPack?.ToString("F2") ?? "";
            }
        }

        private void LoadTypes()
        {
            try
            {
                var types = _repository.GetMaterialTypes();
                cmbType.DataSource = new BindingSource(types, null);
                cmbType.DisplayMember = "Value";
                cmbType.ValueMember = "Key";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить типы материалов.\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите тип материала.", "Ошибка ввода",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите наименование материала.", "Ошибка ввода",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                MessageBox.Show("Цена должна быть числом ≥ 0 (до сотых).", "Ошибка ввода",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUnit.Text))
            {
                MessageBox.Show("Укажите единицу измерения.", "Ошибка ввода",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtStock.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal stock) || stock < 0)
            {
                MessageBox.Show("Количество на складе не может быть отрицательным.", "Ошибка ввода",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!decimal.TryParse(txtMinQuantity.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minQty) || minQty < 0)
            {
                MessageBox.Show("Минимальное количество не может быть отрицательным.", "Ошибка ввода",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPack.Text))
            {
                if (!decimal.TryParse(txtPack.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pack) || pack < 0)
                {
                    MessageBox.Show("Количество в упаковке должно быть числом ≥ 0.", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                Material material = new Material
                {
                    Id = _isEditMode ? _currentMaterial.Id : 0,
                    MaterialTypeId = (int)cmbType.SelectedValue,
                    Name = txtName.Text.Trim(),
                    Price = decimal.Parse(txtPrice.Text.Replace(',', '.'), CultureInfo.InvariantCulture),
                    Unit = txtUnit.Text.Trim(),
                    QuantityInStock = decimal.Parse(txtStock.Text.Replace(',', '.'), CultureInfo.InvariantCulture),
                    MinQuantity = decimal.Parse(txtMinQuantity.Text.Replace(',', '.'), CultureInfo.InvariantCulture),
                    QuantityInPack = string.IsNullOrWhiteSpace(txtPack.Text)
                        ? (decimal?)null
                        : decimal.Parse(txtPack.Text.Replace(',', '.'), CultureInfo.InvariantCulture)
                };

                if (_isEditMode)
                    _repository.UpdateMaterial(material);
                else
                    _repository.InsertMaterial(material);

                MessageBox.Show("Данные успешно сохранены.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении:\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Выйти без сохранения изменений?",
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}