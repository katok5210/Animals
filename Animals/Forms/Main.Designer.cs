namespace Animals
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.domainUpDownWidth = new System.Windows.Forms.DomainUpDown();
            this.domainUpDownHigh = new System.Windows.Forms.DomainUpDown();
            this.domainUpDownQuantityAnimals = new System.Windows.Forms.DomainUpDown();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.WidthAndHigh = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonBegin = new System.Windows.Forms.Button();
            this.radioButtonLow = new System.Windows.Forms.RadioButton();
            this.radioButtonMedium = new System.Windows.Forms.RadioButton();
            this.radioButtonMany = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // domainUpDownWidth
            // 
            this.domainUpDownWidth.Location = new System.Drawing.Point(17, 67);
            this.domainUpDownWidth.Name = "domainUpDownWidth";
            this.domainUpDownWidth.Size = new System.Drawing.Size(120, 20);
            this.domainUpDownWidth.TabIndex = 1;
            this.domainUpDownWidth.Text = "0";
            // 
            // domainUpDownHigh
            // 
            this.domainUpDownHigh.Location = new System.Drawing.Point(243, 67);
            this.domainUpDownHigh.Name = "domainUpDownHigh";
            this.domainUpDownHigh.Size = new System.Drawing.Size(120, 20);
            this.domainUpDownHigh.TabIndex = 2;
            this.domainUpDownHigh.Text = "0";
            // 
            // domainUpDownQuantityAnimals
            // 
            this.domainUpDownQuantityAnimals.Location = new System.Drawing.Point(17, 62);
            this.domainUpDownQuantityAnimals.Name = "domainUpDownQuantityAnimals";
            this.domainUpDownQuantityAnimals.Size = new System.Drawing.Size(120, 20);
            this.domainUpDownQuantityAnimals.TabIndex = 3;
            this.domainUpDownQuantityAnimals.Text = "0";
            // 
            // domainUpDown1
            // 
            this.domainUpDown1.Location = new System.Drawing.Point(17, 56);
            this.domainUpDown1.Name = "domainUpDown1";
            this.domainUpDown1.Size = new System.Drawing.Size(120, 20);
            this.domainUpDown1.TabIndex = 4;
            this.domainUpDown1.Text = "0";
            // 
            // WidthAndHigh
            // 
            this.WidthAndHigh.AutoSize = true;
            this.WidthAndHigh.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WidthAndHigh.Location = new System.Drawing.Point(111, 22);
            this.WidthAndHigh.Name = "WidthAndHigh";
            this.WidthAndHigh.Size = new System.Drawing.Size(164, 24);
            this.WidthAndHigh.TabIndex = 5;
            this.WidthAndHigh.Text = "Ширина и высота";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(13, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(223, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Колличество животных";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(13, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 24);
            this.label2.TabIndex = 7;
            this.label2.Text = "Сколько лет моделировать";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.WidthAndHigh);
            this.groupBox1.Controls.Add(this.domainUpDownWidth);
            this.groupBox1.Controls.Add(this.domainUpDownHigh);
            this.groupBox1.Location = new System.Drawing.Point(34, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 107);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.domainUpDownQuantityAnimals);
            this.groupBox2.Location = new System.Drawing.Point(34, 158);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(379, 104);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.domainUpDown1);
            this.groupBox3.Location = new System.Drawing.Point(34, 278);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(379, 95);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            // 
            // buttonBegin
            // 
            this.buttonBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonBegin.Location = new System.Drawing.Point(34, 528);
            this.buttonBegin.Name = "buttonBegin";
            this.buttonBegin.Size = new System.Drawing.Size(379, 48);
            this.buttonBegin.TabIndex = 11;
            this.buttonBegin.Text = "Начать";
            this.buttonBegin.UseVisualStyleBackColor = true;
            this.buttonBegin.Click += new System.EventHandler(this.buttonBegin_Click);
            // 
            // radioButtonLow
            // 
            this.radioButtonLow.AutoSize = true;
            this.radioButtonLow.Checked = true;
            this.radioButtonLow.Location = new System.Drawing.Point(20, 28);
            this.radioButtonLow.Name = "radioButtonLow";
            this.radioButtonLow.Size = new System.Drawing.Size(52, 17);
            this.radioButtonLow.TabIndex = 12;
            this.radioButtonLow.TabStop = true;
            this.radioButtonLow.Text = "Мало";
            this.radioButtonLow.UseVisualStyleBackColor = true;
            // 
            // radioButtonMedium
            // 
            this.radioButtonMedium.AutoSize = true;
            this.radioButtonMedium.Location = new System.Drawing.Point(20, 51);
            this.radioButtonMedium.Name = "radioButtonMedium";
            this.radioButtonMedium.Size = new System.Drawing.Size(62, 17);
            this.radioButtonMedium.TabIndex = 13;
            this.radioButtonMedium.TabStop = true;
            this.radioButtonMedium.Text = "Средне";
            this.radioButtonMedium.UseVisualStyleBackColor = true;
            // 
            // radioButtonMany
            // 
            this.radioButtonMany.AutoSize = true;
            this.radioButtonMany.Location = new System.Drawing.Point(20, 74);
            this.radioButtonMany.Name = "radioButtonMany";
            this.radioButtonMany.Size = new System.Drawing.Size(57, 17);
            this.radioButtonMany.TabIndex = 14;
            this.radioButtonMany.TabStop = true;
            this.radioButtonMany.Text = "Много";
            this.radioButtonMany.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonLow);
            this.groupBox4.Controls.Add(this.radioButtonMany);
            this.groupBox4.Controls.Add(this.radioButtonMedium);
            this.groupBox4.Location = new System.Drawing.Point(34, 388);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(379, 108);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 596);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.buttonBegin);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Main";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DomainUpDown domainUpDownWidth;
        private System.Windows.Forms.DomainUpDown domainUpDownHigh;
        private System.Windows.Forms.DomainUpDown domainUpDownQuantityAnimals;
        private System.Windows.Forms.DomainUpDown domainUpDown1;
        private System.Windows.Forms.Label WidthAndHigh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonBegin;
        private System.Windows.Forms.RadioButton radioButtonLow;
        private System.Windows.Forms.RadioButton radioButtonMedium;
        private System.Windows.Forms.RadioButton radioButtonMany;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}

