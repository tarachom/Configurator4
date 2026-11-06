<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" indent="yes" encoding="utf-8" />    

    <!-- Файл -->
    <xsl:param name="File" />

    <!-- Простори імен -->
    <xsl:param name="NameSpaceGeneratedCode" />
    <xsl:param name="NameSpace" />

    <xsl:template match="root">

        <xsl:if test="$File = 'TemplateForElementFields'">
            <xsl:call-template name="DocumentTemplateForElementFields" />
        </xsl:if>

    </xsl:template>

    <xsl:template name="DocumentTemplateForElementFields">
        <xsl:variable name="DocumentName" select="Document/Name"/>
        <xsl:variable name="DocumentLatinName" select="Document/LatinName"/>
        <xsl:variable name="FieldsTL" select="Document/ElementFields/Field"/>

        <interface>
            <template class="{concat($DocumentLatinName, '_ElementFields')}" parent="GtkBox">
                <property name="orientation">vertical</property>
                <property name="spacing">10</property>
                <!-- Зовнішні відступи від країв вікна -->
                <property name="margin-start">10</property>
                <property name="margin-end">10</property>
                <property name="margin-top">10</property>
                <property name="margin-bottom">10</property>

                <xsl:for-each select="$FieldsTL">
                    <xsl:variable name="Size">
                        <xsl:choose>
                            <xsl:when test="Size != '0'"><xsl:value-of select="Size"/></xsl:when>
                            <xsl:otherwise>300</xsl:otherwise>
                        </xsl:choose>
                    </xsl:variable>

                    <child>
                        <object class="GtkBox">
                            <property name="orientation">horizontal</property>
                            <property name="spacing">10</property>

                            <child>
                                <object class="GtkLabel">
                                    <property name="label"><xsl:value-of select="Caption"/>:</property>
                                    <property name="halign">start</property>
                                </object>
                            </child>

                            <xsl:choose>
                                <xsl:when test="Type = 'string'">
                                    <xsl:choose>
                                        <xsl:when test="Multiline = '1'">

                                            <child>
                                                <object class="GtkScrolledWindow">
                                                    <property name="width-request"><xsl:value-of select="$Size"/></property>
                                                    <property name="height-request">50</property> 
                                                    <property name="has-frame">true</property>
                                                    <property name="child">
                                                        <object class="GtkTextView" id="{Name}">
                                                            <property name="wrap-mode">word-char</property>
                                                        </object>
                                                    </property>
                                                </object>
                                            </child>

                                        </xsl:when>
                                        <xsl:otherwise>

                                            <child>
                                                <object class="GtkEntry" id="{Name}">
                                                    <property name="width-request"><xsl:value-of select="$Size"/></property>
                                                </object>
                                            </child>

                                        </xsl:otherwise>
                                    </xsl:choose>
                                </xsl:when>
                                <xsl:when test="Type = 'integer'">
                                    <child>
                                        <object class="IntegerControl" id="{Name}" />
                                    </child>
                                </xsl:when>
                                <xsl:when test="Type = 'numeric'">
                                    <child>
                                        <object class="NumericControl" id="{Name}" />
                                    </child>
                                </xsl:when>
                                <xsl:when test="Type = 'boolean'">
                                    <child>
                                        <object class="GtkCheckButton" id="{Name}">
                                            <property name="label"><xsl:value-of select="Caption"/></property>
                                        </object>
                                    </child>
                                </xsl:when>
                                <xsl:when test="Type = 'date' or Type = 'datetime'">
                                    <child>
                                        <object class="DateTimeControl" id="{Name}" />
                                    </child>
                                </xsl:when>
                                <xsl:when test="Type = 'time'">
                                    <child>
                                        <object class="TimeControl" id="{Name}" />
                                    </child>
                                </xsl:when>
                                <xsl:when test="Type = 'composite_pointer'">
                                    <child>
                                        <object class="CompositePointerControl" id="{Name}" />
                                    </child>
                                </xsl:when>
                                <xsl:when test="Type = 'pointer'">
                                    <child>
                                        <object class="{concat(LatinName, '_PointerControl')}" id="{Name}" />
                                    </child>
                                </xsl:when>
                                <xsl:when test="Type = 'enum'">
                                    <child>
                                        <object class="DropDownControl" id="{Name}" />
                                    </child>
                                </xsl:when>
                            </xsl:choose>

                        </object>
                    </child>

                </xsl:for-each>

            </template>
        </interface>

    </xsl:template>

</xsl:stylesheet>