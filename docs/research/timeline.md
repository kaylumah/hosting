<!-- <section class="py-8 px-6 sm:px-8">
    <div class="mx-auto">
        <h2 class="font-architect text-2xl">Timeline</h2>
        <div class="flow-root">
            <ul class="-mb-8">
                {% for event in site.data.events %}
                <li>
                    <div class="relative pb-8">
                        {% unless forloop.last %}
                        <span class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200" aria-hidden="true"></span>
                        {% endunless %}
                        <div class="relative flex space-x-3">
                            <div class="fa-color">
                                <span class="h-8 w-8 rounded-full bg-gray-200 flex items-center justify-center ring-4 ring-white">
                                    <em class="{{ event.icon }}"></em>
                                </span>
                            </div>
                            <div class="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                                <div>
                                    <p>{{ event.name }}</p>
                                </div>
                                <div class="text-right text-sm whitespace-nowrap text-gray-500">
                                <time datetime="2020-09-20">Sep 20</time>
                                </div>
                            </div>
                        </div>
                    </div>
                </li>
                {% endfor %}
            </ul>
        </div>
    </div>
</section> -->