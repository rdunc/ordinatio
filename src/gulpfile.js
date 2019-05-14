var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var del = require('del');
 
gulp.task('clean', function() {
  return del([
	'wwwroot/assets/build/js', 
	'wwwroot/assets/build/css'
  ]);
});
 
gulp.task('js', ['clean'], function() {
  return gulp.src([
	  'wwwroot/assets/js/vendor/jquery.min.js',
      'wwwroot/assets/js/vendor/bootstrap.min.js',
    ])
    .pipe(sourcemaps.init())
    .pipe(concat('bundle.js'))
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('wwwroot/assets/build/js'));
});

gulp.task('sass', ['clean'], function () {
    return gulp.src([
      'wwwroot/assets/css/vendor/*.scss',
      'wwwroot/assets/css/*.scss',
    ])
    .pipe(sass().on('error', sass.logError))
    .pipe(concat('bundle.css'))
    .pipe(gulp.dest('wwwroot/assets/build/css'));
});
 
gulp.task('default', function () {
	gulp.watch('wwwroot/assets/css/*.*', ['sass', 'js']);
});