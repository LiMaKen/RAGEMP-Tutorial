<template>
  <div class="hud">
    <div class="crosshair" v-if="crosshairshow && crosshair > 0">
        <img :src="getCrosshair(crosshair)" alt="Crosshair">
    </div>
    <div id="container">
        <progress-bar type="circle" ref="healthbar" color="#e03a3a" strokeWidth="0.5" duration="2000" :options="options1">
        </progress-bar>
    </div>
    <div id="container2">
        <progress-bar type="circle" ref="hungerbar" color="#a8450c" strokeWidth="0.5" duration="2000" :options="options2">
        </progress-bar>
    </div>
    <div id="container3">
        <progress-bar type="circle" ref="thirstbar" color="#0c6aa8" strokeWidth="0.5" duration="2000" :options="options3">
        </progress-bar>
    </div>
  </div>
</template>

<script>
import Vue from "vue";
import VueProgress from 'vue-progress';
Vue.use(VueProgress);

export default {
  name: 'HUD',
  data() {
      return {
          crosshairshow: false,
          crosshair: 0,
          sound: null,
          options1: {
              color: '#e03a3a',
              strokeWidth: 7.0,
              warnings: false,
              svgStyle: {
                  width: '100%',
                  height: '100%'
              },
              text: {
                  value: '&#10084;'
              }
          },
          options2: {
              color: '#a8450c',
              strokeWidth: 7.0,
              warnings: false,
              svgStyle: {
                  width: '100%',
                  height: '100%'
              },
              text: {
                  value: '&#127828;'
              }
          },
          options3: {
              color: '#0c6aa8',
              strokeWidth: 7.0,
              warnings: false,
              svgStyle: {
                  width: '100%',
                  height: '100%'
              },
              text: {
                  value: '&#127865;'
              }
          }
      }
  },
  methods: {
      getCrosshair(crosshair) {
        return require('../assets/images/crosshair/' + crosshair + '.png')
      },
      showCrosshair: function (crosshair) {
        this.crosshairshow = true;
        this.crosshair = crosshair;
      },
      hideCrosshair: function () {
        this.crosshairshow = false;
      },
      playSound: function(music) {
        if(this.sound != null)
        {
            this.sound.stop();
        }
        else
        {
            this.sound = new Audio('../assets/sounds/'+music);
            this.sound.volume = 1;
            this.sound.play();
        }
      },
      updateProgressbar: function(bar, wert) {
          if(bar == 1)
          {
              this.$refs.healthbar.animate(wert);
          }
          if(bar == 2)
          {
              this.$refs.hungerbar.animate(wert);
          }
          if(bar == 3)
          {
              this.$refs.thirstbar.animate(wert);
          }
      }
  },
}
</script>

<style scoped>
.crosshair {
    position: absolute;
    right: 50%;
    top: 50%;
    transform: translate(50%, -50%);
}
#container {
    margin-top: 787px;
    margin-left: 370px;
    padding: 5px;
    width: 50px;
    height: 50px;
    clear: left;
}
#container2 {
    margin-left: 370px;
    padding: 5px;
    width: 50px;
    height: 50px;
    clear: left;
}
#container3 {
    margin-left: 370px;
    padding: 5px;
    width: 50px;
    height: 50px;
    clear: left;
}
</style>
